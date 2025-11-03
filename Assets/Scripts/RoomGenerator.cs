using UnityEngine;
using System.Collections.Generic;

public class RoomGenerator : MonoBehaviour
{
    [System.Serializable]
    public class RoomPrefabs
    {
        public GameObject normalRoom;
        public GameObject specialRoom;
        public GameObject corridor;
    }

    public RoomPrefabs roomPrefabs;
    public float roomSpacing = 20f;
    public int maxRooms = 100;
    [Header("Entity Spawning")]
    // When a room prefab has an EntitySpawner component (recommended), RoomGenerator will call it.
    public int spawnsPerRoom = 1;
    
    private List<GameObject> generatedRooms = new List<GameObject>();
    private Vector3 lastRoomPosition = Vector3.zero;

    private void Start()
    {
        GenerateInitialRoom();
    }

    public void GenerateNextRoom()
    {
        // Determine room type (normal, special, or corridor)
        float random = Random.value;
        GameObject roomPrefab;

        if (random < 0.1f) // 10% chance for special room
        {
            roomPrefab = roomPrefabs.specialRoom;
        }
        else if (random < 0.4f) // 30% chance for corridor
        {
            roomPrefab = roomPrefabs.corridor;
        }
        else // 60% chance for normal room
        {
            roomPrefab = roomPrefabs.normalRoom;
        }

        // Calculate next room position
        Vector3 nextPosition = lastRoomPosition + Vector3.forward * roomSpacing;
        
        // Instantiate the room
        GameObject newRoom = Instantiate(roomPrefab, nextPosition, Quaternion.identity);
        generatedRooms.Add(newRoom);
        lastRoomPosition = nextPosition;

        // If the room prefab contains EntitySpawner components, trigger spawning
        for (int i = 0; i < spawnsPerRoom; i++)
        {
            EntitySpawner[] spawners = newRoom.GetComponentsInChildren<EntitySpawner>();
            foreach (var sp in spawners)
            {
                if (sp != null)
                    sp.SpawnRandomForRoom();
            }
        }

        // Clean up old rooms if too many exist
        if (generatedRooms.Count > maxRooms)
        {
            Destroy(generatedRooms[0]);
            generatedRooms.RemoveAt(0);
        }
    }

    private void GenerateInitialRoom()
    {
        GameObject initialRoom = Instantiate(roomPrefabs.normalRoom, Vector3.zero, Quaternion.identity);
        generatedRooms.Add(initialRoom);
        lastRoomPosition = Vector3.zero;
        // trigger spawners in initial room as well
        EntitySpawner[] spawners = initialRoom.GetComponentsInChildren<EntitySpawner>();
        foreach (var sp in spawners)
        {
            if (sp != null)
                sp.SpawnRandomForRoom();
        }
    }
}