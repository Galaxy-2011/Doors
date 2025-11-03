using UnityEngine;
using System.Collections;

public class JackEntity : Entity
{
    public float lifeTime = 3f;
    public float jumpForce = 5f;
    public float attackRange = 1.5f;

    private bool hasAttacked = false;

    protected override void Start()
    {
        base.Start();
        entityName = "Jack";
        moveSpeed = 10f;
        damageAmount = 20f;
        detectionRange = 12f;
        StartCoroutine(LifeCycle());
    }

    private IEnumerator LifeCycle()
    {
        float elapsed = 0f;
        while (elapsed < lifeTime && GameManager.Instance.isGameActive)
        {
            if (player != null)
            {
                Vector3 dir = (player.position - transform.position).normalized;
                transform.position += dir * moveSpeed * Time.deltaTime;
                transform.LookAt(player);

                if (!hasAttacked && Vector3.Distance(transform.position, player.position) < attackRange)
                {
                    OnPlayerContact();
                    hasAttacked = true;
                }
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    protected override void OnPlayerContact()
    {
        base.OnPlayerContact();
        // Brief panic effect: reduce player's sprint speed
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null)
        {
            StartCoroutine(TemporarySpeedReduction(pc, 4f));
        }
    }

    private IEnumerator TemporarySpeedReduction(PlayerController pc, float duration)
    {
        float originalSprint = pc.sprintSpeed;
        pc.sprintSpeed *= 0.6f;
        yield return new WaitForSeconds(duration);
        if (pc != null)
            pc.sprintSpeed = originalSprint;
    }
}
