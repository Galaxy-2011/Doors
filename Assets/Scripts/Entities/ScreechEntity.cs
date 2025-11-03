using UnityEngine;
using System.Collections;

public class ScreechEntity : Entity
{
    public float appearChance = 0.3f;
    public float reactionTimeWindow = 1f;
    public float lookAwayAngle = 45f;
    private bool isActive = false;
    private bool playerReacted = false;

    protected override void Start()
    {
        base.Start();
        entityName = "Screech";
        damageAmount = 35f;
        detectionRange = 10f;
    }

    public void TryAppear()
    {
        if (Random.value < appearChance && !isActive)
        {
            StartCoroutine(ScreechSequence());
        }
    }

    private IEnumerator ScreechSequence()
    {
        isActive = true;
        playerReacted = false;

        // Play screech sound
        PlayEntityEffect("ScreechAppear", 0.18f);

        // Give player time to react
        float reactionTimer = 0f;
        while (reactionTimer < reactionTimeWindow)
        {
            if (IsPlayerLookingAway())
            {
                playerReacted = true;
                break;
            }
            reactionTimer += Time.deltaTime;
            yield return null;
        }

        // Handle result
        if (!playerReacted)
        {
            OnPlayerFailed();
        }

        // Clean up
        isActive = false;
        Destroy(gameObject);
    }

    private bool IsPlayerLookingAway()
    {
        if (player == null) return false;

        Camera playerCamera = player.GetComponentInChildren<Camera>();
        if (playerCamera != null)
        {
            Vector3 directionToScreech = (transform.position - playerCamera.transform.position).normalized;
            float angle = Vector3.Angle(playerCamera.transform.forward, directionToScreech);
            return angle > lookAwayAngle;
        }
        return false;
    }

    private void OnPlayerFailed()
    {
        // Apply damage
        base.OnPlayerContact();

        // Apply visual and audio effects
        VFXManager.Instance?.ScreenDistortion(0.3f, 0.8f);
        AudioManager.Instance?.PlaySound("ScreechJumpscare");
        
        // Add trauma for screen shake
        VFXManager.Instance?.AddTrauma(0.6f);
    }
}