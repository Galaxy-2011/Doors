using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI interactionPrompt;
    public TextMeshProUGUI statusEffects;
    public Slider sanityBar;
    public TextMeshProUGUI roomCounter;
    public GameObject gameOverPanel;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            UpdateRoomCounter(GameManager.Instance.currentRoom);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Wire up to player status
        PlayerStatus playerStatus = FindObjectOfType<PlayerStatus>();
        if (playerStatus != null)
        {
            playerStatus.interactionPrompt = interactionPrompt;
            playerStatus.statusEffectText = statusEffects;
            playerStatus.sanityBar = sanityBar;
        }
    }

    public void UpdateRoomCounter(int roomNumber)
    {
        if (roomCounter != null)
        {
            roomCounter.text = $"Room {roomNumber}";
        }
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void RestartGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }
}