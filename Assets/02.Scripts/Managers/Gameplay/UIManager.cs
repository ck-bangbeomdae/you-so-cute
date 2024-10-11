using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private TextMeshProUGUI elapsedTimeText;
    private TextMeshProUGUI godModeText;
    private TextMeshProUGUI flipCountText;
    private TextMeshProUGUI deathCountText;

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;

        elapsedTimeText = transform.Find("Canvas/ElapsedTimeText").GetComponent<TextMeshProUGUI>();
        godModeText = transform.Find("Canvas/[DEBUG]/GodModeText").GetComponent<TextMeshProUGUI>();
        flipCountText = transform.Find("Canvas/[DEBUG]/FlipCountText").GetComponent<TextMeshProUGUI>();
        deathCountText = transform.Find("Canvas/[DEBUG]/DeathCountText").GetComponent<TextMeshProUGUI>();
    }

    public void UpdateElapsedTime(string elapsedTime)
    {
        elapsedTimeText.text = $"Time : {elapsedTime}";
    }

    public void UpdateGodModeText(bool isGodMode)
    {
        godModeText.text = $"God Mode : {(isGodMode ? "true" : "false")}";
    }

    public void UpdateFlipCountText(int flipCount)
    {
        flipCountText.text = $"Flip : {flipCount}";
    }

    public void UpdateDeathCountText(int deathCount)
    {
        deathCountText.text = $"Death : {deathCount}";
    }
}
