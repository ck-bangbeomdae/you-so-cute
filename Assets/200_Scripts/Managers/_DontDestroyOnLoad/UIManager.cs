using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private TextMeshProUGUI elapsedTimeText;
    private Slider progressPortalCountSlider;

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;

        elapsedTimeText = transform.Find("Canvas/ElapsedTimeText").GetComponent<TextMeshProUGUI>();
        progressPortalCountSlider = transform.Find("Canvas/ProgressPortalCountSlider").GetComponent<Slider>();
    }

    public void UpdateElapsedTime(string elapsedTime)
    {
        elapsedTimeText.text = elapsedTime;
    }

    public void UpdateProgressPortalCount(int maxProgressPortalCount, int currentProgressPortalCount)
    {
        progressPortalCountSlider.value = currentProgressPortalCount / maxProgressPortalCount;
    }
}
