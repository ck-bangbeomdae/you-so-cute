using UnityEngine;

public class Goal : MonoBehaviour, ICollisionable
{
    [SerializeField] private SceneTransition sceneTransition;

    public void OnCollision(Player player)
    {
        GameplayManager.Instance.isGameRunning = false;

        // 기록 업데이트
        UpdatePlayerRecord();

        // 진행사항 초기화
        InitializeProgress();

        // 프로필 저장
        ProfileManager.Instance.SaveProfile();

        TransitionManager.Instance.LoadScene(sceneTransition);
    }

    private void UpdatePlayerRecord()
    {
        var record = new Record(
            ProfileManager.Instance.playerProfile.playerName,
            GameplayManager.Instance.ElapsedTime,
            GameplayManager.Instance.CoinCount,
            GameplayManager.Instance.FlipCount,
            GameplayManager.Instance.DeathCount
        );
        ProfileManager.Instance.UpdateRecord(record);
    }

    private void InitializeProgress()
    {
        GameplayManager.Instance.playerSavepoint = new PlayerSpawnpoint();
        GameplayManager.Instance.ElapsedTime = 0f;
        GameplayManager.Instance.CoinCount = 0;
        GameplayManager.Instance.FlipCount = 0;
        GameplayManager.Instance.DeathCount = 0;
        ProfileManager.Instance.playerProfile.progressSave = new ProgressSave();
    }
}
