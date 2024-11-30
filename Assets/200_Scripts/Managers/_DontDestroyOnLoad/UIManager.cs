using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject canvas;
    [SerializeField] private TextMeshProUGUI elapsedTimeText;
    [SerializeField] private Slider progressPortalCountSliderTop;
    [SerializeField] private Slider progressPortalCountSliderBottom;
    [SerializeField] private GameObject progressPortalCountIcon;
    [SerializeField] private List<Transform> pathObjects = new List<Transform>();
    [SerializeField] private GameObject manual;

    private List<Vector3> pathPoints = new List<Vector3>();
    private Vector2 newManualTargetTransform;

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;

        manual.SetActive(true);
        newManualTargetTransform = manual.transform.position;
        manual.transform.position = new Vector2(-420f, manual.transform.position.y);
    }

    private void Start()
    {
        foreach (Transform pathObject in pathObjects)
        {
            pathPoints.Add(pathObject.transform.position);
        }
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
        float halfMaxCount = maxProgressPortalCount / 2f;
        float normalizedCount = currentProgressPortalCount / halfMaxCount;

        progressPortalCountSliderTop.DOValue(Mathf.Min(1f, (normalizedCount * 0.9f) + 0.1f), 0.5f);
        progressPortalCountSliderBottom.DOValue(Mathf.Min(1f, Mathf.Max(0f, (currentProgressPortalCount - halfMaxCount) / halfMaxCount) * 0.9f), 0.5f);
        progressPortalCountIcon.transform.DOMove(GetPointOnPath((float)currentProgressPortalCount / maxProgressPortalCount), 0.5f);

        if (isDecreasing)
        {
            // TODO : 역주행시 효과 추가
        }
    }

    public void OpenManual()
    {
        manual.transform.DOMoveX(newManualTargetTransform.x, 1.5f).SetEase(Ease.OutBack);
    }

    public void CloseManual()
    {
        manual.transform.DOMoveX(-420f, 1f).SetEase(Ease.InOutBack);
    }

    private Vector3 GetPointOnPath(float t)
    {
        if (pathPoints == null || pathPoints.Count < 2)
        {
            Debug.LogError("Path points가 충분하지 않습니다.");
            return Vector3.zero;
        }

        // t 값을 0~1 사이로 클램프
        t = Mathf.Clamp01(t);

        // 전체 경로 길이 계산
        float totalLength = 0f;
        List<float> segmentLengths = new List<float>();

        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            float segmentLength = Vector3.Distance(pathPoints[i], pathPoints[i + 1]);
            segmentLengths.Add(segmentLength);
            totalLength += segmentLength;
        }

        // t에 해당하는 전체 거리 계산
        float targetDistance = t * totalLength;

        // 각 구간을 확인하며 타겟 거리 찾기
        float cumulativeLength = 0f;

        for (int i = 0; i < segmentLengths.Count; i++)
        {
            float segmentLength = segmentLengths[i];

            if (cumulativeLength + segmentLength >= targetDistance)
            {
                // 구간 내에서의 상대적 t 계산
                float segmentT = (targetDistance - cumulativeLength) / segmentLength;
                return Vector3.Lerp(pathPoints[i], pathPoints[i + 1], segmentT);
            }

            cumulativeLength += segmentLength;
        }

        // 만약 정확히 마지막 점에 해당한다면
        return pathPoints[pathPoints.Count - 1];
    }
}
