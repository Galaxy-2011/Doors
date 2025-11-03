using UnityEngine;
using System.IO;

[System.Serializable]
public class GameSaveData
{
    public int currentRoom;
    public float playerSanity;
    public Vector3 playerPosition;
    public Quaternion playerRotation;
    public SerializableInventory inventory;
    public float playTime;
    public int deathCount;
    public string[] unlockedAchievements;
}

[System.Serializable]
public class SerializableInventory
{
    public string[] itemNames;
    public int[] itemCounts;
}

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }
    
    private string SavePath => Path.Combine(Application.persistentDataPath, "doorsGame.save");
    private GameSaveData currentSave;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGame(); // Load any existing save (JSON)
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame()
    {
        GameSaveData saveData = new GameSaveData();
        
        // Get GameManager data
        if (GameManager.Instance != null)
        {
            saveData.currentRoom = GameManager.Instance.currentRoom;
            saveData.playerSanity = GameManager.Instance.playerSanity;
        }

        // Get player data
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            saveData.playerPosition = player.transform.position;
            saveData.playerRotation = player.transform.rotation;

            // Save inventory
            Inventory inv = player.GetComponent<Inventory>();
            if (inv != null)
            {
                var items = inv.GetItems();
                saveData.inventory = new SerializableInventory
                {
                    itemNames = new string[items.Count],
                    itemCounts = new int[items.Count]
                };

                for (int i = 0; i < items.Count; i++)
                {
                    saveData.inventory.itemNames[i] = items[i].itemName;
                    // If you track item counts, add them here
                    saveData.inventory.itemCounts[i] = 1;
                }
            }
        }

        // Save achievements
        if (AchievementManager.Instance != null)
        {
            var list = AchievementManager.Instance.achievements;
            var unlocked = new System.Collections.Generic.List<string>();
            foreach (var a in list)
            {
                if (a.unlocked) unlocked.Add(a.id);
            }
            saveData.unlockedAchievements = unlocked.ToArray();
        }

        // Save to file
        try
        {
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(SavePath, json);
            currentSave = saveData;
            Debug.Log($"Game saved successfully to {SavePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving game: {e.Message}");
        }
    }

    public void LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("No save file found");
            return;
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            currentSave = JsonUtility.FromJson<GameSaveData>(json);

            // Apply loaded data
            if (GameManager.Instance != null)
            {
                GameManager.Instance.currentRoom = currentSave.currentRoom;
                GameManager.Instance.playerSanity = currentSave.playerSanity;
            }

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && currentSave != null)
            {
                player.transform.position = currentSave.playerPosition;
                player.transform.rotation = currentSave.playerRotation;

                // Restore inventory
                if (currentSave.inventory != null)
                {
                    Inventory inv = player.GetComponent<Inventory>();
                    if (inv != null)
                    {
                        // Clear current inventory
                        var currentItems = inv.GetItems();
                        foreach (var item in currentItems)
                        {
                            inv.RemoveItem(item);
                        }

                        // Add saved items
                        for (int i = 0; i < currentSave.inventory.itemNames.Length; i++)
                        {
                            // You'll need to implement item creation based on name
                            CreateAndAddItem(currentSave.inventory.itemNames[i], inv);
                        }
                    }
                }
            }

            Debug.Log($"Game loaded successfully from {SavePath}");
            // Apply loaded achievements
            if (currentSave != null && AchievementManager.Instance != null && currentSave.unlockedAchievements != null)
            {
                foreach (var id in currentSave.unlockedAchievements)
                {
                    AchievementManager.Instance.UnlockAchievement(id, false);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading game: {e.Message}");
        }
    }

    private void CreateAndAddItem(string itemName, Inventory inventory)
    {
        // Create item based on name - implement this based on your item system
        GameObject itemPrefab = Resources.Load<GameObject>($"Items/{itemName}");
        if (itemPrefab != null)
        {
            GameObject newItem = Instantiate(itemPrefab);
            Item item = newItem.GetComponent<Item>();
            if (item != null)
            {
                inventory.AddItem(item);
            }
        }
    }

    public bool HasSaveGame()
    {
        return File.Exists(SavePath);
    }

    public void DeleteSaveGame()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            currentSave = null;
            Debug.Log("Save file deleted");
        }
    }
}