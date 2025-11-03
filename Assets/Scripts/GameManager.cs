using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public int currentRoom = 0;
    public bool isGameActive = false;
    public float playerSanity = 100f;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartGame(bool loadSave = false)
    {
        if (loadSave && SaveSystem.Instance != null && SaveSystem.Instance.HasSaveGame())
        {
            SaveSystem.Instance.LoadGame();
        }
        else
        {
            isGameActive = true;
            currentRoom = 0;
            playerSanity = 100f;
            // Play initial music/ambience
            AudioManager.Instance?.PlayMusic("MainTheme");
            AudioManager.Instance?.PlayAmbience("RoomAmbience");
        }
    }

    public void GameOver()
    {
        isGameActive = false;
        GameUI ui = FindObjectOfType<GameUI>();
        if (ui != null)
        {
            ui.ShowGameOver();
        }
    }

    public void NextRoom()
    {
        currentRoom++;
        GameUI ui = FindObjectOfType<GameUI>();
        if (ui != null)
        {
            ui.UpdateRoomCounter(currentRoom);
        }
        // Trigger room generation
        // Check simple achievement milestones
        if (currentRoom == 10)
        {
            AchievementManager.Instance?.UnlockAchievement("reach_room_10");
        }
    }
}