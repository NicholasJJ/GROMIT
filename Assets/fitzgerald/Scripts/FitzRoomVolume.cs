using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FitzRoomVolume : MonoBehaviour
{
    public string roomId;
    private BoxCollider box;
    private List<EnemySpawner> spawners = new List<EnemySpawner>();
    [SerializeField] List<GameObject> roomObjects;
    
    public static List<FitzRoomVolume> PointToRooms(Vector3 point) {
        var rooms = new List<FitzRoomVolume>();
        foreach (var room in FindObjectsOfType<FitzRoomVolume>()) {
            if (room.ContainsPoint(point)) {
                rooms.Add(room);
            }
        }
        return rooms;
    }

    void Awake() {
        box = GetComponent<BoxCollider>();
    }

    void Update(){
    }

    public Vector3 RandomPointInRoom() {
        return new Vector3(
            Random.Range(box.bounds.min.x+.5f, box.bounds.max.x-.5f),
            Random.Range(box.bounds.min.y + .5f, box.bounds.max.y - .5f),
            Random.Range(box.bounds.min.z + .5f, box.bounds.max.z - .5f)
        );
    }

    public bool ContainsPoint(Vector3 worldPos)
    {
        // Bad hack
        if (!box) { box = GetComponent<BoxCollider>(); }
        Vector3 localPos = box.transform.InverseTransformPoint(worldPos);
        Vector3 delta = localPos - box.center + box.size * 0.5f;
        return Vector3.Max(Vector3.zero, delta) == Vector3.Min(delta, box.size);
    }

    public void RegisterEnemySpawner(EnemySpawner spawner) {
        spawners.Add(spawner);
    }

    public void ClearEnemies() {
        foreach (var spawner in spawners) {
            spawner.ClearSpawnedEnemy();
        }
    }

    public void Load() {
        foreach (var roomObject in roomObjects) {
            try
            {
                RecursiveHide(roomObject, true);
            }
            catch
            {

            }
        }
    }

    public void Unload() {
        Unload(new List<GameObject>());
    }

    public void Unload(List<GameObject> ignore) {
        foreach (var roomObject in roomObjects) {
            if (ignore.Contains(roomObject)) {
                continue;
            }
            try
            {
                RecursiveHide(roomObject, false);
            } catch (MissingReferenceException e)
            {
                Debug.Log($"Missing Reference for object");

            }
            
        }
    }

    public void RoomEntered(FitzPlayer player) {
        foreach (var spawner in spawners) {
            spawner.SpawnEnemy();
        }
        Load();
    }

    public void RoomExited(FitzPlayer player) {
        var ignoreList = new List<GameObject>();
        foreach (var room in player.currentRooms) {
            if (room != this) {
                ignoreList.AddRange(room.roomObjects);
            }
        }
        Unload(ignoreList);
        ClearEnemies();
    }

    public void AddRoomObject(GameObject obj) {
        roomObjects.Add(obj);
    }

    public void RemoveRoomObject(GameObject obj) {
        roomObjects.Remove(obj);
    }

    void RecursiveHide(GameObject obj, bool show)
    {
        foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>())
            if (renderer) renderer.enabled = show;
    }
}
