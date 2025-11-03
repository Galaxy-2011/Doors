using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Achievement
{
    public string id;
    public string title;
    public string description;
    public bool unlocked = false;
}

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }

    public List<Achievement> achievements = new List<Achievement>();

    private void Awake()
    {
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

    private void Start()
    {
        // Example: add a simple achievement if none exist
        if (achievements.Count == 0)
        {
            achievements.Add(new Achievement { id = "reach_room_10", title = "Tenacious", description = "Reach room 10" });
        }
    }

    public void UnlockAchievement(string id, bool save = true)
    {
        var ach = achievements.Find(a => a.id == id);
        if (ach != null && !ach.unlocked)
        {
            ach.unlocked = true;
            Debug.Log($"Achievement unlocked: {ach.title}");

            // Persist achievements via SaveSystem (could extend SaveData)
            if (save)
                SaveSystem.Instance?.SaveGame();

            // Play sound and UI feedback
            AudioManager.Instance?.PlaySound("AchievementUnlocked");
            VFXManager.Instance?.ScreenDistortion(0.4f, 0.7f);
        }
    }

    public bool IsUnlocked(string id)
    {
        var ach = achievements.Find(a => a.id == id);
        return ach != null && ach.unlocked;
    }
}
