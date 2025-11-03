using UnityEngine;
using System.Collections;

public class FigureEntity : Entity
{
    public float maxStalkDistance = 30f;
    public float minStalkDistance = 15f;
    public float disappearTime = 3f;
    private bool isDisappearing = false;

    protected override void Start()
    {
        base.Start();
        entityName = "Figure";
        moveSpeed = 4f;
        damageAmount = 30f;
        detectionRange = 40f;
    }

    protected override void Update()
    {
        base.Update();
        if (!isActive || player == null || isDisappearing) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Stalk the player from a distance
        if (distanceToPlayer > maxStalkDistance)
        {
            // Move closer to player
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
        else if (distanceToPlayer < minStalkDistance)
        {
            // Back away from player
            Vector3 direction = (transform.position - player.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }

        // Always look at player
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        // Check if player is looking at Figure
        if (IsPlayerLookingAtFigure())
        {
            StartCoroutine(DisappearSequence());
        }
    }

    private bool IsPlayerLookingAtFigure()
    {
        Camera playerCamera = player.GetComponentInChildren<Camera>();
        if (playerCamera != null)
        {
            Vector3 directionToFigure = (transform.position - playerCamera.transform.position).normalized;
            float dotProduct = Vector3.Dot(playerCamera.transform.forward, directionToFigure);
            return dotProduct > 0.85f; // Player is looking roughly at Figure
        }
        return false;
    }

    private IEnumerator DisappearSequence()
    {
        if (!isDisappearing)
        {
            isDisappearing = true;
            
            // Fade out effect could be implemented here
            // TODO: Add fade out effect

            yield return new WaitForSeconds(disappearTime);
            
            // Teleport to a new position far from player's view
            Vector3 newPosition = FindNewPosition();
            transform.position = newPosition;
            
            isDisappearing = false;
        }
    }

    private Vector3 FindNewPosition()
    {
        // Find a position behind the player
        Vector3 behindPlayer = player.position - player.forward * maxStalkDistance;
        behindPlayer += Random.insideUnitSphere * 10f; // Add some randomness
        return behindPlayer;
    }

    protected override void OnPlayerContact()
    {
        base.OnPlayerContact();
        StartCoroutine(DisappearSequence());
    }
}