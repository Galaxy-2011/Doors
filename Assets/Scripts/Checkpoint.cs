using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    public bool autoSaveOnEnter = true;
    public string checkpointName;

    private void Reset()
    {
        Collider c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Save position and room info via SaveSystem
        if (autoSaveOnEnter && SaveSystem.Instance != null)
        {
            // update save data with current player position
            SaveSystem.Instance.SaveGame();
            Debug.Log($"Checkpoint reached: {checkpointName}");
        }
    }
}