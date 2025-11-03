using UnityEngine;
using System.Collections;

public class HaltEntity : Entity
{
    public float freezeDuration = 3f;
    public float disappearDelay = 2f;
    private bool hasTriggered = false;

    protected override void Start()
    {
        base.Start();
        entityName = "Halt";
        damageAmount = 10f;
        detectionRange = 15f;
    }

    protected override void Update()
    {
        base.Update();
        if (isActive && !hasTriggered)
        {
            // Check if player is moving
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null && 
                (Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f))
            {
                OnPlayerMovement();
            }
        }
    }

    private void OnPlayerMovement()
    {
        if (!hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(HaltSequence());
        }
    }

    private IEnumerator HaltSequence()
    {
        // Play halt warning sound
        PlayEntityEffect("HaltAppear", 0.15f);

        yield return new WaitForSeconds(0.5f);

        // Freeze player
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Apply damage
        base.OnPlayerContact();

        // Wait for freeze duration
        yield return new WaitForSeconds(freezeDuration);

        // Unfreeze player
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        // Wait before disappearing
        yield return new WaitForSeconds(disappearDelay);

        // Destroy self
        Destroy(gameObject);
    }
}