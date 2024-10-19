using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CombinerManager : Singleton<CombinerManager>
{
    string lastRequestKey = "";
    Dictionary<string, CombinerObjectData> likedRequests;
    string path;


    [SerializeField] GameObject combinerPrefab;
    private static string startingPrompt = "As part of a Unity game, the player is able to combine objects together to make new object. So combining " +
        "a fire and a stick might make a flaming sword, and combining a leaf and smoke might make wind. Each object has an associated " +
        "function that that can be used in combat. So a fireball might damage all enemies close to the player, or wind might move the player forward " +
        "a few feet. For each of the following pairs of objects, your response should be a JSON of the form {'display':display, 'description': description, 'code': code}, " +
        "where display is the hexcode of an emoji that represents the object, " +
        "description is a 1-2 word plaintext descriptions of the interaction, " +
        "and code is a method that will be called by the object to preform it's associated action. " +
        "Your code can involve multiple functions and IEnumerators, but the main function must be called public override void Action(). " +
        "The entire interaction should be within the static method, do not reference other scripts that might not exist within the project." +
        "\n" +
        "only return the json, no other text!" +
        "\n" +
        "Your code can add components to objects, but keep these limited to the basic components that come with unity. " +
        "The only other component you can use is 'DamageOnCollision' which damages enemies when they hit the collider attatched to the gameobject. " +
        "Use AddComponent<DamageOnCollision>().damage = X to set the damage. Since most spells are meant to attack enemies, you should use DamageOnCollision for most spells!" +
        "Also, to create a particle system gameobject use Instantiate(particleSystem)." +
        "\n\n" +
        "the combined spells should make thematic sense, but don't need to match the capabilities of the original spells perfects. For example if a spell" +
        "moves the player around, the new spell can also move them but maybe in a different way";

    [SerializeField] ActiveItem player;
    OutputTextManager outputTextManager;

    public string savedRequests;

    Dictionary<string, CombinerObjectData> pastRequests;
    Dictionary<string, System.Type> codeToBehaviours = new Dictionary<string, System.Type>();

    Queue<(CombinerObject, CombinerObject)> requestQueue = new Queue<(CombinerObject, CombinerObject)>();
    private bool processingRequest = false;

    private HashSet<string> examples = new HashSet<string>();

    private CombinerObject a;
    private CombinerObject b;

    // Start is called before the first frame update
    void Start()
    {
        path = Application.persistentDataPath + "/data.txt";
        string startingPath = Application.persistentDataPath + "/starting.txt";
        if (File.Exists(startingPath))
        {
            savedRequests = File.ReadAllText(startingPath);
            Debug.Log("Found starting path");
        }
        pastRequests = LoadSerializedResponses(savedRequests);
        likedRequests = LoadSerializedResponses(savedRequests);
        Debug.Log($"path={path}");
        outputTextManager = OutputManager.Instance.gameObject.GetComponent<OutputTextManager>();
        if (!ChatGPTWrapper.CustomGPT.Instance.GetStartingPrompt().Equals(startingPrompt))
            ChatGPTWrapper.CustomGPT.Instance.ResetChat(startingPrompt, new List<string>());
    }

    public void AddExample(CombinerObjectData example)
    {
        if (examples.Contains(example.description)) return;
        examples.Add(example.description);

        if (!ChatGPTWrapper.CustomGPT.Instance.GetStartingPrompt().Equals(startingPrompt))
            ChatGPTWrapper.CustomGPT.Instance.ResetChat(startingPrompt, new List<string>());
        ChatGPTWrapper.CustomGPT.Instance.appendMessages($"give the full json of an object whose description is '{example.description}'",
            JsonUtility.ToJson(example));
    }

    public void Combine(CombinerObject o1, CombinerObject o2)
    {
        player.ForceDrop();
        string key = DictKey(o1, o2);
        if (pastRequests.ContainsKey(key))
        {
            CompileResponse(pastRequests[key],o1,o2);
        }
        else
        {
            //ChatGPTWrapper.CustomGPT.Instance.SendToChatGPTAsSystem(InteractionPrompt(a, b), CompileResponseString);
            //Timer.Instance.Begin();
            if (!AutoGenerator.Instance.enabled)
            {
                o1.gameObject.SetActive(false);
                o2.gameObject.SetActive(false);
            }
            requestQueue.Enqueue((o1, o2));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!processingRequest && requestQueue.Count > 0)
        {
            processingRequest = true;
            (a, b) = requestQueue.Dequeue();
            if (a != null && b != null)
            {
                string key = DictKey(a, b);
                if (pastRequests.ContainsKey(key))
                {
                    CompileResponse(pastRequests[key]);
                } else
                {
                    ChatGPTWrapper.CustomGPT.Instance.SendToChatGPTAsSystem(InteractionPrompt(a, b), CompileResponseString);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.L) && lastRequestKey.Length > 0)
        {
            likedRequests[lastRequestKey] = pastRequests[lastRequestKey];
            PrintSerializedResponses(likedRequests);
        }
    }

    void CompileResponseString(string response)
    {
        CombinerObjectData output = JsonUtility.FromJson<CombinerObjectData>(OutputManager.ReplaceCurvedQuotes(response));
        CompileResponse(output);
    }

    void CompileResponse(CombinerObjectData output, CombinerObject o1 = null, CombinerObject o2 = null)
    {
        if (o1 == null)
        {
            o1 = a;
            o2 = b;
        }
        GameObject combined = Instantiate(combinerPrefab);
        try
        {
            if (codeToBehaviours.ContainsKey(output.code))
            {
                combined.AddComponent(codeToBehaviours[output.code]);
            } else
            {
                System.Type t = OutputManager.Instance.GetComponent<CompilerManager>().CombinerCompiler(combined, output.code);
                combined.AddComponent(t);
                codeToBehaviours[output.code] = t;
            }
        }
        catch (CustomCompilerError e)
        {
            Destroy(combined);
            Timer.Instance.retries += 1;
            Debug.Log("Compilation failed! trying again");
            string redoPrompt = $"the code you gave failed to compile due to the following error: {e} \n try again, and remember to only use components that already exist!" +
                $"Remember, use Gameobject particle = Instantiate(particleSys) to create partical systems, then something like var main = particle.GetComponent<ParticleSystem>().main; main.startColor = ... to edit the particle system. And use a collider along with AddComponent<DamageOnCollision>().damage = X to add damage on collisions";
            ChatGPTWrapper.CustomGPT.Instance.SendToChatGPTAsSystem(redoPrompt, CompileResponseString);
            return;
        }
        combined.GetComponent<CombinerObject>().Setup(output);
        combined.transform.position = o1.transform.position;
        combined.transform.parent = o1.transform.parent;
        outputTextManager.DisplayText($"{o1.data.description} and {o2.data.description} were combined to make {combined.GetComponent<CombinerObject>().data.description}");
        string key = DictKey(o1, o2);
        if (!pastRequests.ContainsKey(key))
            pastRequests[key] = output;
        if (AutoGenerator.Instance.enabled) AutoGenerator.Instance.NewObject(combined.GetComponent<CombinerObject>(), key);
        if (!AutoGenerator.Instance.enabled)
        {
            Destroy(o1.gameObject);
            Destroy(o2.gameObject);
        }
        PrintSerializedResponses();
        processingRequest = false;
        lastRequestKey = key;
    }

    static string InteractionPrompt(CombinerObject a, CombinerObject b)
    {
        return $"Combine {a.data.description} with {b.data.description}\n" +
            $"For reference, {a.data.description} has this code: {a.data.code}\n and {b.data.description} has this code: {b.data.code}\n\n" +
            $"Remember, use Instantiate(particleSys) to create partical systems and use a collider along with AddComponent<DamageOnCollision>().damage = X to add damage on collisions";
    }



    public static string DictKey(CombinerObject a, CombinerObject b)
    {
        string o1ID = a.data.description;
        string o2ID = b.data.description;
        return DictKey(o1ID, o2ID);
    }

    private static string DictKey(string o1ID, string o2ID)
    {
        var sorted = SortedStrings(o1ID, o2ID);
        return $"({sorted.Item1},{sorted.Item2})";
    }

    private static (string, string) SortedStrings(string s1, string s2)
    {
        return (s1.CompareTo(s2) <= 0) ? (s1, s2) : (s2, s1);
    }

    private void PrintSerializedResponses(Dictionary<string, CombinerObjectData> requestResponses = null)
    {
        if (requestResponses == null) requestResponses = pastRequests;
        Dictionary<string, string> dict = new Dictionary<string, string>();
        foreach (var k in requestResponses.Keys)
        {
            dict[k] = JsonUtility.ToJson(requestResponses[k]);
        }
        Debug.Log("data:");
        string currentData = JsonUtility.ToJson(new SerializableStringDictionary(dict));
        Debug.Log(currentData);
        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine(currentData);
        writer.Close();
        Timer.Instance.Display();
    }

    private Dictionary<string,CombinerObjectData> LoadSerializedResponses(string responses)
    {
        try
        {
            Dictionary<string, string> sDict = JsonUtility.FromJson<SerializableStringDictionary>(responses).ToDictionary();
            Dictionary<string, CombinerObjectData> dict = new Dictionary<string, CombinerObjectData>();
            foreach (var k in sDict.Keys)
            {
                dict[k] = JsonUtility.FromJson<CombinerObjectData>(sDict[k]);
            }
            return dict;
        } catch
        {
            return new Dictionary<string, CombinerObjectData>();
        }
        
    }
}
