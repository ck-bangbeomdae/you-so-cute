using TMPro;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    // 컴포넌트
    [SerializeField] private SceneTransition newGameSceneTransition;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private TextMeshProUGUI lastRecordText;
    [SerializeField] private GameObject top10HighRecords;

    private void Start()
    {
        UpdatePlayerName();
        UpdateLastRecord();
        UpdateTop10HighScores();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            Record newRecord = new Record();
            newRecord.playerName = "JH";
            newRecord.elapsedTime = Random.Range(100, 10000);
            newRecord.coinCount = 100;
            newRecord.flipCount = Random.Range(10, 100);
            newRecord.deathCount = 100;
            SettingsManager.Instance.SaveRecord(newRecord);

            UpdateTop10HighScores();
        }
    }

    public void OnPlayerNameChanged(string newValue)
    {
        SettingsManager.Instance.playerProfile.playerName = newValue;
        SettingsManager.Instance.SaveSettings();
    }

    public void OnClickNewGameButton()
    {
        TransitionManager.Instance.LoadScene(newGameSceneTransition);
    }

    public void OnClickContinueButton()
    {
        if (!string.IsNullOrEmpty(SettingsManager.Instance.playerProfile.playerSpawnpoint.sceneTransition.sceneName))
        {
            TransitionManager.Instance.LoadSceneWithPlayer(SettingsManager.Instance.playerProfile.playerSpawnpoint);
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
        playerNameInputField.text = SettingsManager.Instance.playerProfile.playerName;
    }

    private void UpdateLastRecord()
    {
        Record lastRecord = SettingsManager.Instance.playerProfile.lastRecord;
        lastRecordText.text = lastRecord.flipCount == 0 ? "NO RECORD" : StringUtils.FormatRecord(lastRecord);
    }

    private void UpdateTop10HighScores()
    {
        Record[] top10HighRecords = SettingsManager.Instance.playerProfile.top10HighRecord;
        for (int i = 0; i < top10HighRecords.Length; i++)
        {
            TextMeshProUGUI recordText = this.top10HighRecords.transform.GetChild(i).Find("RecordText").GetComponent<TextMeshProUGUI>();
            recordText.text = top10HighRecords[i].flipCount == 0 ? "NO RECORD" : StringUtils.FormatRecord(top10HighRecords[i]);
        }
    }
}
