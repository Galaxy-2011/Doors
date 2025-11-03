using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Centralizes player status effects and provides a clean API for entities/items to affect the player.
/// </summary>
public class PlayerStatus : MonoBehaviour
{
    public static PlayerStatus Instance { get; private set; }

    [Header("UI References")]
    public TMPro.TextMeshProUGUI interactionPrompt;
    public TMPro.TextMeshProUGUI statusEffectText;
    public UnityEngine.UI.Slider sanityBar;

    [Header("Effect Settings")]
    public float defaultEffectDuration = 3f;
    public float promptFadeSpeed = 2f;

    private PlayerController playerController;
    private Dictionary<string, float> activeEffects = new Dictionary<string, float>();
    private float originalWalkSpeed;
    private float originalSprintSpeed;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            originalWalkSpeed = playerController.walkSpeed;
            originalSprintSpeed = playerController.sprintSpeed;
        }

        // Clear UI
        if (interactionPrompt != null)
            interactionPrompt.text = "";
        if (statusEffectText != null)
            statusEffectText.text = "";
    }

    private void Update()
    {
        UpdateEffects();
        UpdateSanityBar();
    }

    public void ShowInteractionPrompt(string text)
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.text = text;
            interactionPrompt.alpha = 1f;
        }
    }

    public void HideInteractionPrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.text = "";
        }
    }

    public void ApplySpeedModifier(float multiplier, float duration)
    {
        if (playerController == null) return;

        playerController.walkSpeed = originalWalkSpeed * multiplier;
        playerController.sprintSpeed = originalSprintSpeed * multiplier;

        StartCoroutine(ResetSpeedAfterDelay(duration));
    }

    public void ApplyStun(float duration)
    {
        if (playerController == null) return;
        
        playerController.enabled = false;
        StartCoroutine(RemoveStunAfterDelay(duration));
        AddStatusEffect("Stunned", duration);
    }

    public void ApplyEffect(string effectName, float duration = -1)
    {
        if (duration < 0) duration = defaultEffectDuration;
        activeEffects[effectName] = Time.time + duration;
        UpdateStatusText();
    }

    public void RemoveEffect(string effectName)
    {
        if (activeEffects.ContainsKey(effectName))
        {
            activeEffects.Remove(effectName);
            UpdateStatusText();
        }
    }

    private void UpdateEffects()
    {
        List<string> expiredEffects = new List<string>();
        foreach (var effect in activeEffects)
        {
            if (Time.time > effect.Value)
            {
                expiredEffects.Add(effect.Key);
            }
        }

        foreach (var effect in expiredEffects)
        {
            activeEffects.Remove(effect);
        }

        if (expiredEffects.Count > 0)
        {
            UpdateStatusText();
        }
    }

    private void UpdateStatusText()
    {
        if (statusEffectText == null) return;

        if (activeEffects.Count == 0)
        {
            statusEffectText.text = "";
            return;
        }

        string effects = string.Join(", ", activeEffects.Keys);
        statusEffectText.text = effects;
    }

    private void UpdateSanityBar()
    {
        if (sanityBar != null && GameManager.Instance != null)
        {
            sanityBar.value = GameManager.Instance.playerSanity / 100f;
        }
    }

    private IEnumerator ResetSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (playerController != null)
        {
            playerController.walkSpeed = originalWalkSpeed;
            playerController.sprintSpeed = originalSprintSpeed;
        }
    }

    private IEnumerator RemoveStunAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }
}