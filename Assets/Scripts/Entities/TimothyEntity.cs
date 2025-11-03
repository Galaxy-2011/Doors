using UnityEngine;
using System.Collections;

public class TimothyEntity : Entity
{
    public float followDistance = 5f;
    public float goldCoinChance = 0.3f;
    public int maxCoinsDropped = 3;
    public float disappearDistance = 15f;
    
    private bool isFollowing = false;
    private int coinsDropped = 0;

    protected override void Start()
    {
        base.Start();
        entityName = "Timothy";
        moveSpeed = 4f;
        damageAmount = 0f; // Timothy doesn't directly damage the player
        detectionRange = 8f;
    }

    protected override void Update()
    {
        base.Update();
        if (!isActive || player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange && !isFollowing)
        {
            StartFollowing();
        }
        else if (distanceToPlayer > disappearDistance && isFollowing)
        {
            Disappear();
        }
        else if (isFollowing)
        {
            FollowPlayer();
        }
    }

    private void StartFollowing()
    {
        isFollowing = true;
        // Play Timothy's greeting sound
        // TODO: Add sound effect
    }

    private void FollowPlayer()
    {
        if (player == null) return;

        // Calculate target position behind player
        Vector3 targetPosition = player.position - player.forward * followDistance;
        
        // Move towards target position
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        
        // Look at player
        transform.LookAt(player);

        // Chance to drop gold coin
        if (Random.value < goldCoinChance * Time.deltaTime && coinsDropped < maxCoinsDropped)
        {
            DropGoldCoin();
        }
    }

    private void DropGoldCoin()
    {
        coinsDropped++;
        
        // Create gold coin at Timothy's position
        // TODO: Instantiate gold coin prefab
        // Add coin drop sound effect and small VFX
        PlayEntityEffect("CoinDrop", 0.08f);
    }

    private void Disappear()
    {
        isFollowing = false;
        
        // Play disappear sound/effect
        // TODO: Add disappear effect

        Destroy(gameObject);
    }

    public void OnPlayerInteract()
    {
        // Timothy might give a special response or drop a coin when player interacts
        if (Random.value < goldCoinChance * 2f && coinsDropped < maxCoinsDropped)
        {
            DropGoldCoin();
        }
    }

    protected override void OnPlayerContact()
    {
        // Timothy doesn't harm the player on contact
        // Could add friendly interaction here
    }
}