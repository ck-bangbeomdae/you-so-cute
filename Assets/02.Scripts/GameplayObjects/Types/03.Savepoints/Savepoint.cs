using UnityEngine.SceneManagement;

public class Savepoint : BasePlayerSpawnpoint, ICollisionable
{
    private void OnValidate()
    {
        playerSpawnpoint.sceneTransition.sceneName = SceneManager.GetActiveScene().name;
        playerSpawnpoint.spawnPosition = transform.position;
    }

    public void OnCollision(Player player)
    {
        // 세이브 파일 생성 및 덮어쓰기
        SettingsManager.Instance.playerProfile.playerSpawnpoint = playerSpawnpoint;
        SettingsManager.Instance.SaveSettings();

        // 세이브 포인트 변경
        GameplayManager.Instance.playerSavepoint = playerSpawnpoint;
    }
}
