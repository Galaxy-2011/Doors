using UnityEngine;
using System.Collections;

public class RushEntity : Entity
{
    public float rushSpeed = 15f;
    public float rushCooldown = 5f;
    private bool isRushing = false;

    protected override void Start()
    {
        base.Start();
        entityName = "Rush";
        moveSpeed = 3f;
        damageAmount = 100f; // Instant kill
    }

    protected override void OnPlayerDetected()
    {
        if (!isRushing)
        {
            StartCoroutine(RushAttack());
        }
    }

    private IEnumerator RushAttack()
    {
        isRushing = true;
        
        // Play warning sound/effect
        // TODO: Add sound effect here

        yield return new WaitForSeconds(1.5f); // Warning time

        // Rush toward player
        float rushDuration = 2f;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = player.position;

        while (elapsedTime < rushDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / rushDuration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        // Cooldown
        yield return new WaitForSeconds(rushCooldown);
        isRushing = false;
    }

    protected override void OnPlayerContact()
    {
        if (isRushing)
        {
            base.OnPlayerContact();
            // Visual and audio feedback
            VFXManager.Instance?.ScreenDistortion(0.5f, 1f);
            AudioManager.Instance?.PlaySound("RushKill");
            // Game over - Rush caught the player
            GameManager.Instance.GameOver();
        }
    }
}