using UnityEngine;
using System.Collections.Generic;

public class HidingSpot : MonoBehaviour
{
    public bool isOccupied = false;
    public float enterExitSpeed = 1f;
    public Vector3 playerHidePosition;
    public Vector3 playerHideRotation;
    
    private PlayerController hiddenPlayer;
    private PlayerController nearbyPlayer;
    public KeyCode interactKey = KeyCode.E;
    public string interactPrompt = "Press E to hide";

    public bool CanHide()
    {
        return !isOccupied;
    }

    public void Hide(PlayerController player)
    {
        if (!CanHide()) return;

        isOccupied = true;
        hiddenPlayer = player;
        
        // Put player into hidden state instead of disabling the component
        player.SetHidden(true);

        // Move player to hide position
        StartCoroutine(MovePlayerToHidePosition(player));
    }

    private System.Collections.IEnumerator MovePlayerToHidePosition(PlayerController player)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = player.transform.position;
        Quaternion startRotation = player.transform.rotation;
        
        while (elapsedTime < enterExitSpeed)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / enterExitSpeed;
            
            player.transform.position = Vector3.Lerp(startPosition, transform.position + playerHidePosition, t);
            player.transform.rotation = Quaternion.Lerp(startRotation, Quaternion.Euler(playerHideRotation), t);
            
            yield return null;
        }
    }

    public void Unhide()
    {
        if (!isOccupied || hiddenPlayer == null) return;

        StartCoroutine(UnhidePlayer());
    }

    private System.Collections.IEnumerator UnhidePlayer()
    {
        Vector3 exitPosition = transform.position + transform.forward * 1.5f;
        Vector3 startPosition = hiddenPlayer.transform.position;
        Quaternion startRotation = hiddenPlayer.transform.rotation;
        
        float elapsedTime = 0f;
        while (elapsedTime < enterExitSpeed)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / enterExitSpeed;
            
            hiddenPlayer.transform.position = Vector3.Lerp(startPosition, exitPosition, t);
            hiddenPlayer.transform.rotation = Quaternion.Lerp(startRotation, Quaternion.identity, t);
            
            yield return null;
        }

        // Re-enable player controller
        hiddenPlayer.SetHidden(false);
        
        isOccupied = false;
        hiddenPlayer = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            nearbyPlayer = other.GetComponent<PlayerController>();
            if (nearbyPlayer != null)
            {
                PlayerStatus.Instance?.ShowInteractionPrompt(interactPrompt);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (nearbyPlayer != null && other.GetComponent<PlayerController>() == nearbyPlayer)
            {
                nearbyPlayer = null;
                PlayerStatus.Instance?.HideInteractionPrompt();
            }
        }
    }

    private void Update()
    {
        if (nearbyPlayer == null) return;

        // If player presses interact and the spot is free, hide
        if (Input.GetKeyDown(interactKey))
        {
            if (!isOccupied)
            {
                Hide(nearbyPlayer);
            }
            else if (hiddenPlayer == nearbyPlayer)
            {
                Unhide();
            }
        }
    }
}