using UnityEngine;
using System.Collections;

public class SeekEntity : Entity
{
    public float chaseSpeed = 7f;
    public float chaseDuration = 20f;
    public float minSpawnDistance = 15f;
    private bool isChasing = false;

    protected override void Start()
    {
        base.Start();
        entityName = "Seek";
        moveSpeed = 6f;
        damageAmount = 40f;
        detectionRange = 20f;
    }

    public void InitiateSeekChase()
    {
        if (!isChasing)
        {
            StartCoroutine(ChaseSequence());
        }
    }

    private IEnumerator ChaseSequence()
    {
        isChasing = true;
        float elapsedTime = 0f;

        // Play seek spawn sound
        PlayEntityEffect("SeekSpawn", 0.2f);

        while (elapsedTime < chaseDuration && GameManager.Instance.isGameActive)
        {
            if (player != null)
            {
                // Calculate direction to player
                Vector3 direction = (player.position - transform.position).normalized;
                
                // Move towards player
                transform.position += direction * chaseSpeed * Time.deltaTime;
                transform.LookAt(player);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // End chase sequence
        Deactivate();
        Destroy(gameObject);
    }

    protected override void OnPlayerContact()
    {
        if (isChasing)
        {
            base.OnPlayerContact();
            // Reduce player speed or apply other effects
            PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            if (player != null)
            {
                player.walkSpeed *= 0.8f;
                player.sprintSpeed *= 0.8f;
            }
        }
    }
}