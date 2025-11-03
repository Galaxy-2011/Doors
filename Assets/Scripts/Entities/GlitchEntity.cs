using UnityEngine;
using System.Collections;

public class GlitchEntity : Entity
{
    public float teleportInterval = 0.7f;
    public int teleportCount = 5;
    public float teleportRadius = 6f;
    public float sanityDrainPerSecond = 10f;
    public float disableDurationAfterGlitch = 2f;
    public bool stunnedPlayerOnContact = true;

    private bool isGlitching = false;

    protected override void Start()
    {
        base.Start();
        entityName = "Glitch";
        moveSpeed = 2f;
        damageAmount = 20f;
        detectionRange = 25f;
    }

    protected override void OnPlayerDetected()
    {
        if (!isGlitching)
        {
            StartCoroutine(GlitchSequence());
        }
    }

    private IEnumerator GlitchSequence()
    {
        isGlitching = true;

        // Optional: play spawn sound / visual
        // TODO: trigger VFX here

        for (int i = 0; i < teleportCount; i++)
        {
            if (player == null) break;

            // Teleport to a random position near the player
            Vector3 randomOffset = Random.onUnitSphere;
            randomOffset.y = Mathf.Abs(randomOffset.y) * 0.5f; // keep roughly level
            Vector3 targetPos = player.position + randomOffset.normalized * Random.Range(2f, teleportRadius);

            transform.position = targetPos;
            transform.LookAt(player);

            // Play a short glitch sfx each teleport
            PlayEntityEffect("GlitchTeleport", 0.12f);

            // Briefly drain sanity while close to player
            float drainTime = teleportInterval;
            float elapsed = 0f;
            while (elapsed < drainTime)
            {
                if (Vector3.Distance(transform.position, player.position) < detectionRange)
                {
                    GameManager.Instance.playerSanity -= sanityDrainPerSecond * Time.deltaTime;
                }
                elapsed += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(0.05f);
        }

        // Brief cooldown where Glitch disappears/is inactive
        yield return new WaitForSeconds(disableDurationAfterGlitch);

        // End of sequence - destroy or deactivate
        Deactivate();
        Destroy(gameObject);
        isGlitching = false;
    }

    protected override void OnPlayerContact()
    {
        base.OnPlayerContact();
        if (stunnedPlayerOnContact)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                // Briefly disable movement to simulate stun
                StartCoroutine(TemporarilyDisablePlayer(pc, 1.5f));
            }
        }
    }

    private IEnumerator TemporarilyDisablePlayer(PlayerController pc, float duration)
    {
        pc.enabled = false;
        yield return new WaitForSeconds(duration);
        if (pc != null)
            pc.enabled = true;
    }
}
