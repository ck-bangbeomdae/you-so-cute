using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private GameObject canvas;
    private TextMeshProUGUI elapsedTimeText;
    private Slider progressPortalCountSlider;

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;

        canvas = transform.Find("Canvas").gameObject;
        elapsedTimeText = transform.Find("Canvas/ElapsedTimeText").GetComponent<TextMeshProUGUI>();
        progressPortalCountSlider = transform.Find("Canvas/ProgressPortalCountSlider").GetComponent<Slider>();
    }

    public void ToggleGameplayUI(bool isGameRunning)
    {
        canvas.SetActive(isGameRunning);
    }

    public void UpdateElapsedTime(string elapsedTime)
    {
        elapsedTimeText.text = elapsedTime;
    }

    public void UpdateProgressPortalCount(int maxProgressPortalCount, int currentProgressPortalCount, bool isDecreasing)
    {
        progressPortalCountSlider.DOValue((float)currentProgressPortalCount / maxProgressPortalCount, 0.5f);

        if (isDecreasing)
        {
            // TODO : 역주행시 효과 추가
        }
    }
}
