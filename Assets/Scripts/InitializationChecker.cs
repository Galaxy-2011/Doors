using UnityEngine;

/// <summary>
/// Simple runtime checker that ensures required manager singletons exist. It can optionally auto-create placeholder GameObjects.
/// Attach this to a bootstrap GameObject in your scene (e.g. "Bootstrap").
/// </summary>
public class InitializationChecker : MonoBehaviour
{
    [Tooltip("Automatically create missing manager GameObjects (best effort). Components requiring Inspector wiring will still need manual setup.")]
    public bool autoCreateMissing = false;

    private void Start()
    {
        CheckAndWarn<GameManager>("GameManager");
        CheckAndWarn<AudioManager>("AudioManager");
        CheckAndWarn<VFXManager>("VFXManager");
        CheckAndWarn<SaveSystem>("SaveSystem");
        CheckAndWarn<AchievementManager>("AchievementManager");
        CheckAndWarn<GameUI>("GameUI");
        CheckAndWarn<PlayerStatus>("PlayerStatus");
    }

    private void CheckAndWarn<T>(string objectName) where T : MonoBehaviour
    {
        T instance = FindObjectOfType<T>();
        if (instance == null)
        {
            Debug.LogError($"InitializationChecker: Missing required manager/component of type {typeof(T).Name}. Expected GameObject named '{objectName}' with component {typeof(T).Name}.");
            if (autoCreateMissing)
            {
                GameObject go = new GameObject(objectName);
                go.AddComponent<T>();
                DontDestroyOnLoad(go);
                Debug.LogWarning($"InitializationChecker: Created placeholder GameObject '{objectName}' with component {typeof(T).Name}. You may need to wire inspector fields manually.");
            }
        }
    }
}