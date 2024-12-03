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

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            StopCoroutine(Delay(delayInSeconds));
            TransitionManager.Instance.LoadSceneWithPlayer(newGameSceneTransition);
        }
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
        TransitionManager.Instance.LoadSceneWithPlayer(newGameSceneTransition);
    }
}
