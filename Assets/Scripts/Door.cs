using UnityEngine;

public class Door : MonoBehaviour
{
    public int doorId = 0;
    public bool isLocked = false;
    public bool isOpen = false;

    public void Unlock()
    {
        isLocked = false;
        // play unlock sound / animation
    }

    public void Open()
    {
        if (isLocked) return;
        if (isOpen) return;
        isOpen = true;
        // play open animation and notify GameManager to load next room
        GameManager.Instance.NextRoom();
    }

    public void Lock()
    {
        isLocked = true;
    }
}