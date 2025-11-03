using UnityEngine;
using System.Collections;

public class EyesEntity : Entity
{
    public float drainRate = 5f;
    public float checkInterval = 0.5f;
    private bool isDraining = false;

    protected override void Start()
    {
        base.Start();
        entityName = "Eyes";
        damageAmount = 0f; // Eyes doesn't directly damage, it drains sanity
        detectionRange = 20f;
        StartCoroutine(CheckPlayerGaze());
    }

    private IEnumerator CheckPlayerGaze()
    {
        while (isActive)
        {
            if (IsPlayerLookingAtEyes())
            {
                if (!isDraining)
                {
                    StartCoroutine(DrainSanity());
                }
            }
            else
            {
                isDraining = false;
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }

    private bool IsPlayerLookingAtEyes()
    {
        if (player == null) return false;

        Camera playerCamera = player.GetComponentInChildren<Camera>();
        if (playerCamera != null)
        {
            Vector3 directionToEyes = (transform.position - playerCamera.transform.position).normalized;
            float dotProduct = Vector3.Dot(playerCamera.transform.forward, directionToEyes);
            
            // Check if player is looking at Eyes
            if (dotProduct > 0.9f) // More strict viewing angle for Eyes
            {
                RaycastHit hit;
                if (Physics.Raycast(playerCamera.transform.position, directionToEyes, out hit))
                {
                    return hit.collider.gameObject == gameObject;
                }
            }
        }
        return false;
    }

    private IEnumerator DrainSanity()
    {
        isDraining = true;
        while (isDraining && GameManager.Instance.playerSanity > 0)
        {
            GameManager.Instance.playerSanity -= drainRate * Time.deltaTime;

            // Visual/Audio feedback
            VFXManager.Instance?.AddTrauma(0.01f);
            PlayEntityEffect("EyesDrain", 0.03f);

            yield return null;
        }
    }

    protected override void OnPlayerContact()
    {
        // Eyes doesn't do anything on contact
    }
}