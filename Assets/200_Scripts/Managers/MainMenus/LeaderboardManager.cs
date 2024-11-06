using Newtonsoft.Json;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderboardManager : MonoBehaviour
{
    // 컴포넌트
    [SerializeField] private TextMeshProUGUI lastRecordText;
    [SerializeField] private GameObject highScores;
    [SerializeField] private GameObject recordPrefab;

    private void Start()
    {
        GameplayManager.Instance.isGameRunning = false;
        StartCoroutine(ScoreAPIUtils.GetScoresCoroutine(OnGetScoresSuccess, OnGetScoresError));
    }

    public void OnClickReturnToTitleButton()
    {
        SceneManager.LoadScene("Scene_Title");
    }

    private void UpdateLastRecord()
    {
        Record lastRecord = ProfileManager.Instance.playerProfile.lastRecord;
        lastRecordText.text = lastRecord.flipCount == 0 ? "NO RECORD" : StringUtils.FormatRecord(lastRecord);
    }

    private void OnGetScoresSuccess(string jsonResponse)
    {
        Record[] records = JsonConvert.DeserializeObject<Record[]>(jsonResponse);

        Array.Sort(records, (x, y) => x.elapsedTime.CompareTo(y.elapsedTime));

        for (int i = 0; i < records.Length; i++)
        {
            Record record = records[i];
            GameObject recordInstance = Instantiate(recordPrefab, highScores.transform);
            TextMeshProUGUI recordText = recordInstance.GetComponentInChildren<TextMeshProUGUI>();
            recordText.text = $"{i + 1}. {StringUtils.FormatRecord(record)}";
        }
    }

    private void OnGetScoresError(string error)
    {
        Debug.LogError("GET 요청 실패: " + error);
    }
}
