using UnityEngine;

public class ManualEvent : MonoBehaviour, IResetable
{
    public void HandleReset()
    {
        GameplayManager.Instance.IsGameRunning = true;
        UIManager.Instance.OpenManual();
    }
}
