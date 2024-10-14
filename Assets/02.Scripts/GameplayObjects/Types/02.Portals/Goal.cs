using UnityEngine;

public class Goal : MonoBehaviour, ICollisionable
{
    [SerializeField] private SceneTransition sceneTransition;

    public void OnCollision(Player player)
    {
        var profileManager = ProfileManager.Instance;
        var gameplayManager = GameplayManager.Instance;

        Record record = new Record(
            profileManager.playerProfile.playerName,
            gameplayManager.ElapsedTime,
            gameplayManager.CoinCount,
            gameplayManager.FlipCount,
            gameplayManager.DeathCount
        );

        profileManager.SaveRecord(record);
        TransitionManager.Instance.LoadScene(sceneTransition);
    }
}
