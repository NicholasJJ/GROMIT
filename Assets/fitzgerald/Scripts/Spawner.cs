using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] Transform spawnSpot;
    [SerializeField] List<CombinerObject> spawnSet;
    [SerializeField] bool random;
    private GameObject combinerPrefab;
    private int index = 0;
    Transform spawnedItem;

    // cache the rooms we exist in so we can easily add items to the correct saved object lists
    private List<FitzRoomVolume> parentRooms = new List<FitzRoomVolume>();

    // Start is called before the first frame update
    void Start()
    {
        parentRooms = FitzRoomVolume.PointToRooms(spawnSpot.position);
        combinerPrefab = Resources.Load<GameObject>("EmojiObject");
        spawnedItem = Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnedItem == null || Vector3.Distance(spawnedItem.position,spawnSpot.position) > 2)
        {
            spawnedItem = Spawn();
        }
    }

    Transform Spawn()
    {
        GameObject combined = Instantiate(combinerPrefab);
        CombinerObject co = NextObject();
        combined.AddComponent(co.GetType());
        combined.GetComponent<CombinerObject>().Setup(co.data);
        combined.transform.position = spawnSpot.transform.position;

        foreach (var room in parentRooms) {
            room.AddRoomObject(combined);
        }
        return combined.transform;
    }

    private CombinerObject NextObject()
    {
        var ret = spawnSet[index];
        if (random)
        {
            index = new System.Random().Next(0, spawnSet.Count);
        }
        else
        {
            index = (index + 1) % spawnSet.Count;
        }
        return ret;
    }
}
