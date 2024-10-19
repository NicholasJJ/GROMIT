using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Linq;

[Serializable]
public class InteractionOutput
{
    public string description;
    public string code;

    public override string ToString() => JsonUtility.ToJson(this);
}

[Serializable]
public class SerializableStringDictionary
{
    [SerializeField]
    private List<string> keys = new List<string>();

    [SerializeField]
    private List<string> values = new List<string>();

    public Dictionary<string, string> ToDictionary()
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();

        for (int i = 0; i < keys.Count; i++)
        {
            if (!dictionary.ContainsKey(keys[i]))
            {
                dictionary.Add(keys[i], values[i]);
            }
            else
            {
                Debug.LogWarning($"Duplicate key found: {keys[i]}. Skipping this entry.");
            }
        }

        return dictionary;
    }

    public SerializableStringDictionary(Dictionary<string,string> fromDictionary)
    {
        foreach (string key in fromDictionary.Keys)
        {
            keys.Add(key);
            values.Add(fromDictionary[key]);
        }
    }
}


public class Interaction
{
    private Action<GameObject, GameObject> method;
    public InteractionOutput serializableInteraction;

    public Interaction(InteractionOutput gptOutput)
    {
        serializableInteraction = new InteractionOutput
        {
            code = gptOutput.code,
            description = gptOutput.description
        };
        method = (Action<GameObject, GameObject>) CompilerManager.CompileMethod<Action<GameObject, GameObject>>(gptOutput.code);
    }

    public void Interact(GameObject a, GameObject b)
    {
        Debug.Log($"performing interaction |{serializableInteraction.description}| on gameobjects {a.name} and {b.name}");
        Timer.Instance.DisplayEscape();
        method.Invoke(a, b);
    }

    public Interaction(string description, string code, Action<GameObject,GameObject> method)
    {
        serializableInteraction = new InteractionOutput
        {
            description = description,
            code = code
        };
        this.method = method;
    }
}

public class RuntimeInteractionMaker : Singleton<RuntimeInteractionMaker>
{
    [SerializeField] bool lazyGeneration;
    [SerializeField] OutputTextManager outputTextManager;
    static string startingPrompt = "As part of a Unity game, the player is able to 'interact' different gameobjects together. The effect of interacting two objects" +
        "depends on what those objects are. So Interact(door,key) might use the key to open the door, and Interact(Bomb,Wall) might destroy the wall witht the bomb." +
        "Interactions should always be simple to implement in Unity, like destroying an object or changing it's size.\n" +
        "for each pair of objects, you're response should be a JSON of the form {'description': description, 'code': code}, where description is a plaintext descriptions of the" +
        "interaction, and code is a static method that preforms the interaction between the objects. The entire interaction should be within the static method, do not reference" +
        "other scripts that might not exist within the project." +
        "\n" +
        "only return the json, no other text!";
        //+
        //" Also, use the component 'StationaryKit' as much as possible";

    public static void Nothing(GameObject a, GameObject b)
    {
        return;
    }

    static Interaction nullInteraction = new Interaction("Nothing Happened",
        "public static void Nothing(GameObject a, GameObject b) \n{ \nreturn; \n}", Nothing);

    [SerializeField] string savedInteractions;

    private HashSet<string> objects = new HashSet<string>();
    private Dictionary<string, string> objectDescriptions = new Dictionary<string, string>();

    private Dictionary<string, Interaction> interactions;
    private Dictionary<string, string> interactionScripts;

    public void AddObject(RuntimeInteractionObject runtimeInteractionObject)
    {
        if (interactions == null) interactions = new Dictionary<string, Interaction>();
        if (objects.Contains(runtimeInteractionObject.GetID())) return;
        objects.Add(runtimeInteractionObject.GetID());
        objectDescriptions.Add(runtimeInteractionObject.GetID(), runtimeInteractionObject.GetDescription());
        Debug.Log("Adding object " + runtimeInteractionObject.GetID());
        foreach (var secondObject in runtimeInteractionObject.DevMadeInteractions().Keys)
        {
            Debug.Log($"second object is {secondObject.GetID()}");
            var key = DictKey(runtimeInteractionObject, secondObject);
            Debug.Log($"adding implemented interaction: {key}");
            interactions[key] = runtimeInteractionObject.DevMadeInteractions()[secondObject];
        } 
    }
 
    // Start is called before the first frame update
    void Start()
    {
        if (interactions == null) interactions = new Dictionary<string, Interaction>();
        try
        {

            interactionScripts = JsonUtility.FromJson<SerializableStringDictionary>(savedInteractions).ToDictionary();
        } catch
        {
            interactionScripts = new Dictionary<string, string>();
        }
        GenerateInteractions();
    }

    Queue<string> neededKeys;

    void GenerateInteractions()
    {
        Debug.Log($"old context is: \n {ChatGPTWrapper.CustomGPT.Instance.GetFullContext()}");
        ChatGPTWrapper.CustomGPT.Instance.ResetChat(startingPrompt, new List<string>());
        neededKeys = new Queue<string>();
        var objectArr = objects.ToArray();

        foreach (var key in interactionScripts.Keys)
            CompileResponse(interactionScripts[key], key);

        for (int i = 0; i < objects.Count; i++)
        {
            for (int j = i+1; j < objects.Count; j++)
            {
                var key = DictKey(objectArr[i], objectArr[j]);
                //Debug.Log($"handling {key}");
                if (!interactions.ContainsKey(key))
                {
                    if (!lazyGeneration)
                    {
                        //if the interaction doesn't exist, get chatGPT to make one
                        Debug.Log($"requesting {key}");
                        neededKeys.Enqueue(key);
                    }
                } else
                {
                    //add the interaction to the gpt context so chatGPT can learn from it
                    Debug.Log($"inserting conversation for {key}");
                    ChatGPTWrapper.CustomGPT.Instance.appendMessages(InteractionPrompt(key), interactions[key].serializableInteraction.ToString());
                }
            }
        }
        Debug.Log($"current chat is: \n {ChatGPTWrapper.CustomGPT.Instance.GetFullContext()}");
        RequestNextKey();
    }

    void RequestNextKey()
    {
        
        if (neededKeys.Count < 1)
        {
            Debug.Log(JsonUtility.ToJson(new SerializableStringDictionary(interactionScripts)));
            return;
        }
        var key = neededKeys.Peek();
        if (interactionScripts != null && interactionScripts.ContainsKey(key))
        {
            CompileNextKey(interactionScripts[key]);
        }
        else
        {
            ChatGPTWrapper.CustomGPT.Instance.SendToChatGPTAsSystem(InteractionPrompt(key), CompileNextKey);
        }
    }

    void CompileResponse(string response, string key)
    {
        InteractionOutput output = JsonUtility.FromJson<InteractionOutput>(OutputManager.ReplaceCurvedQuotes(response));
        if (interactionScripts == null) interactionScripts = new Dictionary<string, string>();
        Interaction newInteraction;
        try
        {
            newInteraction = new Interaction(output);
        }
        catch (Exception e)
        {
            //Debug.Log("Compilation failed, using null interaction");
            Debug.Log("Compilation failed! trying again");
            Timer.Instance.retries += 1;
            string redoPrompt = $"the code you gave failed to compile due to the following error: {e} \n try again, and remember to only use components that already exist!";
            ChatGPTWrapper.CustomGPT.Instance.SendToChatGPTAsSystem(redoPrompt,CompileKey);
            //newInteraction = nullInteraction;
            return;
        }

        if (!interactionScripts.ContainsKey(key)) interactionScripts.Add(key, newInteraction.serializableInteraction.ToString());
        interactions.Add(key, newInteraction);
        currKey = null;
    }

    void CompileNextKey(string response)
    {
        Debug.Log($"received response: \n {response}");
        var key = neededKeys.Dequeue();
        CompileResponse(response, key);
        RequestNextKey();
    }

    string currKey;

    bool GenerationFinished() => currKey == null;

    void CompileKey(string response)
    {
        CompileResponse(response, currKey);
    }

    void GenerateKey(string key)
    {
        ChatGPTWrapper.CustomGPT.Instance.SendToChatGPTAsSystem(InteractionPrompt(key), CompileKey);
    }

    IEnumerator InteractObjects(RuntimeInteractionObject o1, RuntimeInteractionObject o2)
    {
        var sorted = SortedStrings(o1.GetID(), o2.GetID());
        Debug.Log($"TIME attempting to interact {sorted.Item1} with {sorted.Item2}");
        Timer.Instance.item1 = sorted.Item1;
        Timer.Instance.item2 = sorted.Item2;
        //Timer.Instance.Begin();
        if (sorted.Item1.Equals(o1.GetID()))
        {
            Debug.Log("parity check passed, proceeding with interaction");
            var key = DictKey(o1, o2);
            if (!interactions.ContainsKey(key))
            {
                yield return new WaitUntil(GenerationFinished);
                currKey = key;
                Debug.Log($"generating lazy interaction for {key}");
                //foreach (var k in interactions.Keys) Debug.Log($"key: {k}");
                GenerateKey(key);
                yield return new WaitUntil(GenerationFinished);
            }
            outputTextManager.DisplayText(interactions[key].serializableInteraction.description);
            interactions[key].Interact(o1.gameObject, o2.gameObject);
            Debug.Log(JsonUtility.ToJson(new SerializableStringDictionary(interactionScripts)));
        }
        yield return null;
    }

    public void Interact(RuntimeInteractionObject o1, RuntimeInteractionObject o2)
    {
        if (!objectDescriptions.ContainsKey(o1.GetID())) objectDescriptions[o1.GetID()] = o1.gameObject.GetComponent<TextDescription>().GetDescription();
        if (!objectDescriptions.ContainsKey(o2.GetID())) objectDescriptions[o2.GetID()] = o2.gameObject.GetComponent<TextDescription>().GetDescription();
        StartCoroutine(InteractObjects(o1, o2));
    }

    private static string DictKey(RuntimeInteractionObject o1, RuntimeInteractionObject o2)
    {
        string o1ID = o1.GetID();
        string o2ID = o2.GetID();
        return DictKey(o1ID, o2ID);
    }

    private static string DictKey(string o1ID, string o2ID)
    {
        var sorted = SortedStrings(o1ID, o2ID);
        return $"({sorted.Item1},{sorted.Item2})";
    }

    private static (string,string) SortedStrings(string s1, string s2)
    {
        return (s1.CompareTo(s2) <= 0) ? (s1, s2) : (s2, s1);
    }

    private string InteractionPrompt(string key)
    {
        var objectPair = key.Substring(1, key.Length - 2).Split(',');
        string o0 = objectPair[0];
        string o1 = objectPair[1];
        string d0 = objectDescriptions[objectPair[0]];
        string d1 = objectDescriptions[objectPair[1]];
        return $"give the interaction for Interact(GameObject {o0}, GameObject {o1}), where {o0} is {d0} and {o1} is {d1}";
    }
}
