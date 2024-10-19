using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoGenerator : Singleton<AutoGenerator>
{
    public CombinerManager combinerManager;
    [SerializeField] private int depth;
    public List<CombinerObject> combinerObjects;
    public Dictionary<CombinerObject, int> combinerObjectToDepth = new Dictionary<CombinerObject, int>();
    public Dictionary<string, int> keyToDepth = new Dictionary<string, int>();
    private Queue<(CombinerObject, CombinerObject)> combinationQueue = new Queue<(CombinerObject, CombinerObject)>();
    private int progress;
    // Start is called before the first frame update
    void Start()
    {
        progress = 0;
        StartCoroutine(DelayCombine());
    }

    public void NewObject(CombinerObject o, string key)
    {
        progress++;
        Debug.Log($"AUTO: got new object {o.data.description} from key {key}, progress is now {progress}");
        combinerObjectToDepth[o] = keyToDepth[key];
        combinerObjects.Add(o);
        for (int i = 0; i < combinerObjects.Count; i++)
        {
            AddToQueue(o, combinerObjects[i]);
        }
        CombineNextPair();
    }

    void CombineNextPair()
    {
        Debug.Log($"AUTO: queue count is {combinationQueue.Count}");
        if (combinationQueue.Count == 0)
        {
            Debug.Log("AUTO: Combiner Queue Empty! ending");
            return;
        }
        else
        {
            (CombinerObject, CombinerObject) pair = combinationQueue.Dequeue();
            combinerManager.Combine(pair.Item1, pair.Item2);
        }
    }

    void AddToQueue(CombinerObject a, CombinerObject b)
    {
        if (NextDepth((a,b)) <= depth)
        {
            Debug.Log($"AUTO: combine {a.data.description} and {b.data.description}");
            combinationQueue.Enqueue((a, b));
        }
    }

    int NextDepth((CombinerObject,CombinerObject) objectPair)
    {
        int d = Mathf.Max(combinerObjectToDepth[objectPair.Item1], combinerObjectToDepth[objectPair.Item2]) + 1;
        keyToDepth[CombinerManager.DictKey(objectPair.Item1, objectPair.Item2)] = d;
        return d;
    }

    IEnumerator DelayCombine()
    {
        yield return new WaitForSeconds(1);
        foreach (CombinerObject o in combinerObjects) combinerObjectToDepth[o] = 0;
        for (int i = 0; i < combinerObjects.Count; i++)
        {
            for (int j = i; j < combinerObjects.Count; j++)
            {
                AddToQueue(combinerObjects[i], combinerObjects[j]);
            }
        }
        CombineNextPair();
    }
}
