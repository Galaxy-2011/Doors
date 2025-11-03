using UnityEngine;
using System.Collections;

public class AmbushEntity : Entity
{
    public float warningDuration = 1.5f;
    public float rushSpeed = 20f;
    public float attackRange = 2f;
    public int maxAmbushCount = 3;
    
    private int ambushCount = 0;
    private bool isAmbushing = false;
    private Vector3 ambushStartPosition;

    protected override void Start()
    {
        base.Start();
        entityName = "Ambush";
        moveSpeed = 8f;
        damageAmount = 45f;
        detectionRange = 30f;
    }

    public void StartAmbushSequence()
    {
        if (!isAmbushing && ambushCount < maxAmbushCount)
        {
            StartCoroutine(AmbushSequence());
        }
    }

    private IEnumerator AmbushSequence()
    {
        isAmbushing = true;
        ambushCount++;

        // Set up ambush position behind player
        SetupAmbushPosition();

        // Warning phase
        PlayEntityEffect("AmbushWarn", 0.2f);
        yield return new WaitForSeconds(warningDuration);

        // Rush phase
        float rushDuration = 1f;
        float elapsedTime = 0f;
        Vector3 targetPosition = player.position;

        while (elapsedTime < rushDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / rushDuration;
            
            // Move towards the player's position
            transform.position = Vector3.Lerp(ambushStartPosition, targetPosition, t);
            
            // Check if we hit the player
            if (Vector3.Distance(transform.position, player.position) < attackRange)
            {
                OnPlayerContact();
                break;
            }

            yield return null;
        }

        // Reset for next ambush
        isAmbushing = false;

        // If we've completed all ambushes, destroy the entity
        if (ambushCount >= maxAmbushCount)
        {
            Destroy(gameObject);
        }
        else
        {
            // Wait before next potential ambush
            yield return new WaitForSeconds(Random.Range(3f, 8f));
        }
    }

    private void SetupAmbushPosition()
    {
        if (player != null)
        {
            // Position behind player
            Vector3 behindPlayer = player.position - player.forward * 10f;
            ambushStartPosition = behindPlayer;
            transform.position = ambushStartPosition;
            transform.LookAt(player);
        }
    }

    protected override void OnPlayerContact()
    {
        if (isAmbushing)
        {
            base.OnPlayerContact();
            
            // Apply knockback effect to player
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                Vector3 knockbackDirection = (player.position - transform.position).normalized;
                // TODO: Implement knockback effect on player
            }
        }
    }
}