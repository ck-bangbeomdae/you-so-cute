using Newtonsoft.Json;
using System;
using TMPro;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    // 컴포넌트
    [SerializeField] private SceneTransition newGameSceneTransition;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private TextMeshProUGUI lastRecordText;
    [SerializeField] private GameObject highScores;
    [SerializeField] private GameObject recordPrefab;

    private void Start()
    {
        GameplayManager.Instance.isGameRunning = false;

        UpdatePlayerName();
        UpdateLastRecord();

        StartCoroutine(ScoreAPIUtils.GetScoresCoroutine(OnGetScoresSuccess, OnGetScoresError));
    }

    public void OnPlayerNameChanged(string newValue)
    {
        ProfileManager.Instance.playerProfile.playerName = newValue;
        ProfileManager.Instance.SaveProfile();
    }

    public void OnClickNewGameButton()
    {
        GameplayManager.Instance.isGameRunning = true;
        TransitionManager.Instance.LoadScene(newGameSceneTransition);
    }

    public void OnClickContinueButton()
    {
        if (!string.IsNullOrEmpty(ProfileManager.Instance.playerProfile.progressSave.playerSpawnpoint.sceneTransition.sceneName))
        {
            GameplayManager.Instance.isGameRunning = true;
            TransitionManager.Instance.LoadSceneWithPlayer(ProfileManager.Instance.playerProfile.progressSave.playerSpawnpoint);
        }
    }

    public void OnClickExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    private void UpdatePlayerName()
    {
        playerNameInputField.text = ProfileManager.Instance.playerProfile.playerName;
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
