using UnityEngine;
using System.Collections;

public class HideEntity : Entity
{
    public float huntDuration = 30f;
    public float warningDuration = 3f;
    public float attackRange = 5f;
    private bool isHunting = false;

    protected override void Start()
    {
        base.Start();
        entityName = "Hide";
        moveSpeed = 4f;
        damageAmount = 100f; // Instant kill if caught
        detectionRange = 25f;
    }

    public void StartHunt()
    {
        if (!isHunting)
        {
            StartCoroutine(HuntSequence());
        }
    }

    private IEnumerator HuntSequence()
    {
        isHunting = true;

        // Warning phase
        // TODO: Add warning effects/sounds
        yield return new WaitForSeconds(warningDuration);

        float huntTimer = 0f;
        while (huntTimer < huntDuration && GameManager.Instance.isGameActive)
        {
            if (player != null)
            {
                // Move towards player
                Vector3 direction = (player.position - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;
                transform.LookAt(player);

                // Check if player is in closet/hiding spot
                if (!IsPlayerHiding())
                {
                    float distanceToPlayer = Vector3.Distance(transform.position, player.position);
                    if (distanceToPlayer <= attackRange)
                    {
                        OnPlayerContact();
                        break;
                    }
                }
            }

            huntTimer += Time.deltaTime;
            yield return null;
        }

        // End hunt sequence
        isHunting = false;
        Destroy(gameObject);
    }

    private bool IsPlayerHiding()
    {
        if (player == null) return false;
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc == null) return false;
        return pc.isHidden;
    }

    protected override void OnPlayerContact()
    {
        if (isHunting && !IsPlayerHiding())
        {
            base.OnPlayerContact();
            // Visual and audio feedback
            VFXManager.Instance?.ScreenDistortion(0.7f, 1.2f);
            AudioManager.Instance?.PlaySound("HideKill");
            GameManager.Instance.GameOver();
        }
    }
}