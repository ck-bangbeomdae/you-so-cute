using System.Collections;
using UnityEngine;

public class EndingManager : MonoBehaviour, IResetable
{
    [SerializeField] private float delayInSeconds = 10f;

    public void HandleReset()
    {
        StartCoroutine(Delay(delayInSeconds));
    }

    IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);

        SceneTransition leaderboardSceneTransition = new SceneTransition { sceneName = "Scene_Leaderboard", transitionType = TransitionType.FadeInOut };
        TransitionManager.Instance.LoadScene(leaderboardSceneTransition);
    }
}
