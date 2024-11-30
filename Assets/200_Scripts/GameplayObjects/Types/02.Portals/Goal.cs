using UnityEngine;

public class Goal : MonoBehaviour, ICollisionable
{
    [SerializeField] private SceneTransition sceneTransition;

    public void OnCollision(Player player)
    {
        GameplayManager.Instance.IsGameRunning = false;

        // 기록 업데이트
        UpdatePlayerRecord();
    }

    private void UpdatePlayerRecord()
    {
        var newRecord = new Record(
            ProfileManager.Instance.playerProfile.playerName,
            GameplayManager.Instance.ElapsedTime,
            GameplayManager.Instance.flipCount,
            GameplayManager.Instance.deathCount
        );

        // 로컬에 마지막 기록 업데이트
        ProfileManager.Instance.playerProfile.progressSave = new ProgressSave();
        ProfileManager.Instance.playerProfile.lastRecord = newRecord;
        ProfileManager.Instance.SaveProfile();

        // 서버에 기록 전송
        StartCoroutine(ScoreAPIUtils.PostScoreCoroutine(newRecord, OnPostScoreSuccess, OnPostScoreError));
    }

    private void OnPostScoreSuccess(string message)
    {
        Debug.Log("Success: " + message);
        ResetProgressAndTransition();
    }

    private void OnPostScoreError(string error)
    {
        Debug.LogError("Error: " + error);
        ResetProgressAndTransition();
    }

    private void ResetProgressAndTransition()
    {
        // 진행사항 초기화
        GameplayManager.Instance.ResetProgress();

        // 메인메뉴로 씬 전환
        TransitionManager.Instance.LoadScene(sceneTransition);
    }
}
