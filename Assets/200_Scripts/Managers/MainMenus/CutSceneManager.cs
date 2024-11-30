using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class CutSceneManager : MonoBehaviour, IResetable
{
    [SerializeField] private float delayInSeconds = 10f;
    [SerializeField] private PlayerSpawnpoint newGameSceneTransition;

    private VideoPlayer videoPlayer;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }

    public void HandleReset()
    {
        videoPlayer.time = 0;
        videoPlayer.Play();

        StartCoroutine(Delay(delayInSeconds));
    }

    IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameplayManager.Instance.IsGameRunning = true;
        UIManager.Instance.OpenManual();
        TransitionManager.Instance.LoadSceneWithPlayer(newGameSceneTransition);
    }
}
