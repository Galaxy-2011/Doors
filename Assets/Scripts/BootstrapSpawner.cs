using UnityEngine;

/// <summary>
/// Runtime bootstrapper that ensures required manager singletons exist. If a prefab is provided it will instantiate it,
/// otherwise it will create a placeholder GameObject and attach the component.
/// Place this on an empty GameObject in any scene (for example: a "Bootstrap" GameObject).
/// </summary>
public class BootstrapSpawner : MonoBehaviour
{
    [Header("Optional manager prefabs (assign prefabs if you have prepared them)")]
    public GameObject gameManagerPrefab;
    public GameObject audioManagerPrefab;
    public GameObject vfxManagerPrefab;
    public GameObject saveSystemPrefab;
    public GameObject achievementManagerPrefab;
    public GameObject gameUIPrefab;
    public GameObject playerStatusPrefab;
    public GameObject initializationCheckerPrefab;

    [Tooltip("If true, create simple placeholders when no prefab is provided (components will be added but inspector wiring is still required).")]
    public bool autoCreatePlaceholders = true;

    private void Awake()
    {
        // Try to ensure each manager exists
        EnsureManager<GameManager>(gameManagerPrefab, "GameManager");
        EnsureManager<AudioManager>(audioManagerPrefab, "AudioManager");
        EnsureManager<VFXManager>(vfxManagerPrefab, "VFXManager");
        EnsureManager<SaveSystem>(saveSystemPrefab, "SaveSystem");
        EnsureManager<AchievementManager>(achievementManagerPrefab, "AchievementManager");
        EnsureManager<GameUI>(gameUIPrefab, "GameUI");
        EnsureManager<PlayerStatus>(playerStatusPrefab, "PlayerStatus");
        EnsureManager<InitializationChecker>(initializationCheckerPrefab, "InitializationChecker");
    }

    private void EnsureManager<T>(GameObject prefab, string defaultName) where T : MonoBehaviour
    {
        // If an instance already exists, nothing to do
        T existing = FindObjectOfType<T>();
        if (existing != null) return;

        if (prefab != null)
        {
            GameObject go = Instantiate(prefab);
            go.name = prefab.name;
            DontDestroyOnLoad(go);
            Debug.Log($"BootstrapSpawner: Instantiated prefab for {typeof(T).Name}: {go.name}");
            return;
        }

        if (!autoCreatePlaceholders)
        {
            Debug.LogError($"BootstrapSpawner: Missing manager {typeof(T).Name} and no prefab assigned. Assign a prefab or enable autoCreatePlaceholders.");
            return;
        }

        // Create placeholder
        GameObject placeholder = new GameObject(defaultName);
        placeholder.AddComponent<T>();
        DontDestroyOnLoad(placeholder);
        Debug.LogWarning($"BootstrapSpawner: Created placeholder GameObject '{defaultName}' with component {typeof(T).Name}. You may need to wire inspector fields manually.");
    }
}
