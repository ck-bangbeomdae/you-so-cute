using Newtonsoft.Json;
using System;
using TMPro;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour, IResetable
{
    // 컴포넌트
    [SerializeField] private TextMeshProUGUI lastRecordNoText;
    [SerializeField] private TextMeshProUGUI lastRecordNameText;
    [SerializeField] private TextMeshProUGUI lastRecordTimeText;
    [SerializeField] private TextMeshProUGUI lastRecordDeadText;

    [SerializeField] private GameObject highScores;
    [SerializeField] private GameObject goldRecordPrefab;
    [SerializeField] private GameObject blueRecordPrefab;

    private int skipCount = 0;
    private bool isFirst = true;

    private void Start()
    {
        HandleReset();
    }

    public void HandleReset()
    {
        // 로컬 기록 초기화
        lastRecordNoText.text = "";
        lastRecordNameText.text = "";
        lastRecordTimeText.text = "";
        lastRecordDeadText.text = "";

        // highScores 자식 인스턴스 전부 삭제
        foreach (Transform child in highScores.transform)
        {
            Destroy(child.gameObject);
        }

        StartCoroutine(ScoreAPIUtils.GetScoresCoroutine(OnGetScoresSuccess, OnGetScoresError));
    }

    public void OnClickReturnToTitleButton()
    {
        if (!TransitionManager.Instance.isTransition)
        {
            SceneTransition titleSceneTransition = new SceneTransition { sceneName = "Scene_Title", transitionType = TransitionType.FadeInOut };
            TransitionManager.Instance.LoadScene(titleSceneTransition);
        }
    }

    private void OnGetScoresSuccess(string jsonResponse)
    {
        skipCount = 0;
        isFirst = true;

        Record[] records = JsonConvert.DeserializeObject<Record[]>(jsonResponse);

        Array.Sort(records, (x, y) => x.elapsedTime.CompareTo(y.elapsedTime));

        Record lastRecord = ProfileManager.Instance.playerProfile.lastRecord;
        Debug.Log($"{lastRecord.playerName}, {lastRecord.deathCount}, {lastRecord.flipCount}, {StringUtils.FormatElapsedTime(lastRecord.elapsedTime)}");

        for (int i = 0; i < records.Length; i++)
        {
            Record record = records[i];

            if (record.elapsedTime > 120)
            {
                if (record.playerName == lastRecord.playerName &&
                    Math.Abs(record.elapsedTime - lastRecord.elapsedTime) < 0.01f &&
                    record.flipCount == lastRecord.flipCount &&
                    record.deathCount == lastRecord.deathCount)
                {
                    lastRecordNoText.text = $"{i + 1 - skipCount}";
                    lastRecordNameText.text = string.IsNullOrEmpty(record.playerName) ? "noname" : record.playerName;
                    lastRecordTimeText.text = $"{StringUtils.FormatElapsedTime(record.elapsedTime)}";
                    lastRecordDeadText.text = $"{record.deathCount}";
                }

                GameObject recordInstance;
                if (isFirst)
                {
                    recordInstance = Instantiate(goldRecordPrefab, highScores.transform);
                    isFirst = false;
                }
                else
                {
                    recordInstance = Instantiate(blueRecordPrefab, highScores.transform);
                }

                TextMeshProUGUI noText = recordInstance.transform.Find("NoText").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI nameText = recordInstance.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI timeText = recordInstance.transform.Find("TimeText").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI deadText = recordInstance.transform.Find("DeadText").GetComponent<TextMeshProUGUI>();
                noText.text = $"{i + 1 - skipCount}";
                nameText.text = string.IsNullOrEmpty(record.playerName) ? "noname" : record.playerName;
                timeText.text = $"{StringUtils.FormatElapsedTime(record.elapsedTime)}";
                deadText.text = $"{record.deathCount}";
            }
            else
            {
                skipCount++;
            }
        }
    }

    private void OnGetScoresError(string error)
    {
        Debug.LogError("GET 요청 실패: " + error);
    }
}
