using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntitySpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnEntry
    {
        public string entityName;
        public GameObject prefab;
        public float weight = 1f;
        public int maxPerRoom = 1;
    }

    public List<SpawnEntry> spawns = new List<SpawnEntry>();
    public int maxActiveEntities = 5;

    private List<Entity> activeEntities = new List<Entity>();

    public void SpawnRandomForRoom()
    {
        if (spawns.Count == 0) return;

        // Choose a spawn entry by weight
        float total = 0f;
        foreach (var s in spawns) total += s.weight;
        float r = Random.value * total;
        float accum = 0f;
        SpawnEntry chosen = spawns[0];
        foreach (var s in spawns)
        {
            accum += s.weight;
            if (r <= accum)
            {
                chosen = s;
                break;
            }
        }

        // Check max constraints
        int existingOfType = 0;
        foreach (var e in activeEntities)
        {
            if (e.entityName == chosen.entityName) existingOfType++;
        }
        if (existingOfType >= chosen.maxPerRoom) return;

        // Instantiate near the spawner
        Vector3 pos = transform.position + Random.insideUnitSphere * 6f;
        pos.y = transform.position.y;
        GameObject go = Instantiate(chosen.prefab, pos, Quaternion.identity);
        Entity ent = go.GetComponent<Entity>();
        if (ent != null)
        {
            ent.Activate();
            activeEntities.Add(ent);
            StartCoroutine(TrackEntity(ent));
        }

        // Enforce active entity cap
        if (activeEntities.Count > maxActiveEntities)
        {
            // Remove oldest
            var oldest = activeEntities[0];
            if (oldest != null) Destroy(oldest.gameObject);
            activeEntities.RemoveAt(0);
        }
    }

    private IEnumerator TrackEntity(Entity e)
    {
        while (e != null)
        {
            yield return null;
        }
        activeEntities.Remove(e);
    }
}