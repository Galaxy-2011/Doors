using UnityEngine;

public class Key : Item
{
    public int doorId; // ID of the door this key opens

    private void Start()
    {
        itemName = "Key";
        description = "Opens a specific door";
        isConsumable = true;
        isEquippable = false;
    }

    public override void Use()
    {
        // Key usage is handled by the door system
    }
}

public class Crucifix : Item
{
    public float protectionRadius = 5f;
    public float durability = 100f;
    private bool isActive = false;

    private void Start()
    {
        itemName = "Crucifix";
        description = "Protects against certain entities";
        isConsumable = false;
        isEquippable = true;
    }

    public override void Use()
    {
        // Toggle crucifix protection
        isActive = !isActive;
    }

    public override void OnEquip()
    {
        isActive = true;
    }

    public override void OnUnequip()
    {
        isActive = false;
    }

    private void Update()
    {
        if (isActive)
        {
            // Check for entities in range
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, protectionRadius);
            foreach (var hitCollider in hitColliders)
            {
                Entity entity = hitCollider.GetComponent<Entity>();
                if (entity != null)
                {
                    // Repel or destroy entity based on type
                    HandleEntity(entity);
                }
            }
        }
    }

    private void HandleEntity(Entity entity)
    {
        // Different entities react differently to the crucifix
        switch (entity.entityName)
        {
            case "Rush":
            case "Ambush":
                entity.Deactivate();
                durability -= 25f;
                break;
            case "Hide":
                durability -= 50f;
                break;
            // Add more entity interactions
        }

        if (durability <= 0)
        {
            // Break the crucifix
            GetComponent<Inventory>()?.RemoveItem(this);
            Destroy(gameObject);
        }
    }
}

public class Vitamins : Item
{
    public float healAmount = 25f;
    public float speedBoostDuration = 10f;
    public float speedBoostMultiplier = 1.5f;

    private void Start()
    {
        itemName = "Vitamins";
        description = "Restores sanity and temporarily increases speed";
        isConsumable = true;
        isEquippable = false;
    }

    public override void Use()
    {
        // Heal player
        GameManager.Instance.playerSanity = Mathf.Min(
            GameManager.Instance.playerSanity + healAmount, 
            100f
        );

        // Apply speed boost
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (player != null)
        {
            StartCoroutine(ApplySpeedBoost(player));
        }
    }

    private System.Collections.IEnumerator ApplySpeedBoost(PlayerController player)
    {
        float originalWalkSpeed = player.walkSpeed;
        float originalSprintSpeed = player.sprintSpeed;

        player.walkSpeed *= speedBoostMultiplier;
        player.sprintSpeed *= speedBoostMultiplier;

        yield return new WaitForSeconds(speedBoostDuration);

        player.walkSpeed = originalWalkSpeed;
        player.sprintSpeed = originalSprintSpeed;
    }
}

public class Flashlight : Item
{
    public float batteryLife = 100f;
    public float batteryDrainRate = 1f;
    public Light spotlight;

    private void Start()
    {
        itemName = "Flashlight";
        description = "Illuminates dark areas and affects certain entities";
        isConsumable = false;
        isEquippable = true;
        spotlight = GetComponentInChildren<Light>();
        spotlight.enabled = false;
    }

    public override void Use()
    {
        if (batteryLife > 0)
        {
            spotlight.enabled = !spotlight.enabled;
        }
    }

    private void Update()
    {
        if (spotlight.enabled)
        {
            batteryLife -= batteryDrainRate * Time.deltaTime;
            if (batteryLife <= 0)
            {
                spotlight.enabled = false;
            }
        }
    }

    public override void OnUnequip()
    {
        spotlight.enabled = false;
    }
}