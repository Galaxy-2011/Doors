using UnityEngine;

public class Lockpick : Item
{
    public float breakChance = 0.35f; // chance to break when used

    private void Start()
    {
        itemName = "Lockpick";
        description = "Can be used to attempt opening locked doors without a key.";
        isConsumable = true;
        isEquippable = false;
    }

    public override void Use()
    {
        // Find closest locked door within range
        Door[] doors = GameObject.FindObjectsOfType<Door>();
        Door closest = null;
        float best = float.MaxValue;
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) return;

        foreach (var d in doors)
        {
            if (d.isLocked)
            {
                float dist = Vector3.Distance(player.position, d.transform.position);
                if (dist < best && dist < 3f)
                {
                    best = dist;
                    closest = d;
                }
            }
        }

        if (closest != null)
        {
            bool success = Random.value > 0.5f;
            if (success)
            {
                closest.Unlock();
            }
            else
            {
                // Chance to break
                if (Random.value < breakChance)
                {
                    // Remove from inventory - this depends on how Inventory holds items
                    Inventory inv = GameObject.FindObjectOfType<Inventory>();
                    inv?.RemoveItem(this);
                    Destroy(gameObject);
                }
            }
        }
    }
}