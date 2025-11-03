using UnityEngine;
using System.Collections;

public class WindowEntity : Entity
{
    public float drainRate = 20f; // sanity per second when stared at
    public float checkInterval = 0.2f;
    public float appearDuration = 6f;
    private bool isActiveWindow = false;

    protected override void Start()
    {
        base.Start();
        entityName = "Window";
        detectionRange = 30f;
    }

    public void AppearAtPosition(Vector3 pos)
    {
        transform.position = pos;
        Activate();
        isActiveWindow = true;
        StartCoroutine(WindowLife());
    }

    private IEnumerator WindowLife()
    {
        float elapsed = 0f;
        while (elapsed < appearDuration && GameManager.Instance.isGameActive)
        {
            if (IsPlayerLookingAtWindow())
            {
                GameManager.Instance.playerSanity -= drainRate * Time.deltaTime;
                // Mild VFX and sound while staring
                VFXManager.Instance?.AddTrauma(0.02f);
                PlayEntityEffect("WindowGaze", 0.06f);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        Deactivate();
        Destroy(gameObject);
    }

    private bool IsPlayerLookingAtWindow()
    {
        if (player == null) return false;
        Camera playerCamera = player.GetComponentInChildren<Camera>();
        if (playerCamera == null) return false;

        Vector3 dir = (transform.position - playerCamera.transform.position).normalized;
        float dot = Vector3.Dot(playerCamera.transform.forward, dir);
        if (dot > 0.98f)
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, dir, out hit, 100f))
            {
                return hit.collider.gameObject == gameObject;
            }
        }
        return false;
    }
}
