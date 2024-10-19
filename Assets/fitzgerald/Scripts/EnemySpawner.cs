using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Transform spawnLocation;
    [SerializeField] GameObject enemyPrefab;
    private GameObject spawnedEnemy;

    public List<FitzRoomVolume> rooms;

    void Start()
    {
        // var rooms = FitzRoomVolume.PointToRooms(GetSpawnLocation().position);
        // // Possible to be in 2 volumes (probably an error), so just use the first
        // if (rooms.Count > 0) {
        //     roomVolume = rooms[0];
        //     roomVolume.RegisterEnemySpawner(this);
        // }
        rooms = FitzRoomVolume.PointToRooms(GetSpawnLocation().position);
        foreach (var room in rooms)
        {
            room.RegisterEnemySpawner(this);
        }
        //SpawnEnemy();
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(GetSpawnLocation().position, "skull.png", true);
    }

    public Transform GetSpawnLocation()
    {
        return spawnLocation ? spawnLocation : transform;
    }

    public void SpawnEnemy()
    {
        if (spawnedEnemy)
        {
            Destroy(spawnedEnemy);
        }
        spawnedEnemy = Instantiate(enemyPrefab, GetSpawnLocation().position, Quaternion.identity);
        var enemy = spawnedEnemy.GetComponents<FitzEnemy>();
        foreach (var e in enemy)
        {
            e.OnSpawn(this);
        }
    }

    public void ClearSpawnedEnemy()
    {
        Destroy(spawnedEnemy);
    }
}
