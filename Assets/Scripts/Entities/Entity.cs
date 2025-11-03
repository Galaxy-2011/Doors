using UnityEngine;
using System.Collections;

public abstract class Entity : MonoBehaviour
{
    [Header("Entity Settings")]
    public string entityName;
    public float moveSpeed = 5f;
    public float detectionRange = 10f;
    public float damageAmount = 25f;
    
    protected Transform player;
    protected bool isActive = false;

    protected virtual void Start()
    {
        var go = GameObject.FindGameObjectWithTag("Player");
        if (go != null)
            player = go.transform;
        else
        {
            Debug.LogWarning($"Entity ({name}) could not find a GameObject tagged 'Player' at Start(). Some behaviors will be disabled until a Player is present.");
        }
    }

    protected virtual void Update()
    {
        if (!isActive || player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange)
        {
            OnPlayerDetected();
        }
    }

    protected virtual void OnPlayerDetected()
    {
        // Override this in specific entity classes
    }

    public virtual void Activate()
    {
        isActive = true;
    }

    public virtual void Deactivate()
    {
        isActive = false;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerContact();
        }
    }

    protected virtual void OnPlayerContact()
    {
        // Override this in specific entity classes
        GameManager.Instance.playerSanity -= damageAmount;
    }

    /// <summary>
    /// Convenience helper to play a named SFX and trigger a small screen effect.
    /// Entities can call this to keep audio/vfx calls consistent.
    /// </summary>
    protected void PlayEntityEffect(string sfxName = null, float vfxIntensity = 0f)
    {
        if (!string.IsNullOrEmpty(sfxName))
            AudioManager.Instance?.PlaySoundAtPosition(sfxName, transform.position);

        if (vfxIntensity > 0f)
            VFXManager.Instance?.ScreenDistortion(vfxIntensity, 0.6f);
    }
}