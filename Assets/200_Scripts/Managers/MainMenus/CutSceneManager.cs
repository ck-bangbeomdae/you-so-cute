using FMOD.Studio;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class CutSceneManager : MonoBehaviour, IResetable
{
    [SerializeField] private float delayInSeconds = 10f;
    [SerializeField] private PlayerSpawnpoint newGameSceneTransition;

    private VideoPlayer videoPlayer;
    private EventInstance bgmSoundInstance;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }

    private void Update()
    {
        // 컷 씬 건너뛰기
        if (Input.GetButtonDown("Cancel"))
        {
            bgmSoundInstance.stop(STOP_MODE.IMMEDIATE);
            StopCoroutine(Delay(delayInSeconds));
            TransitionManager.Instance.LoadSceneWithPlayer(newGameSceneTransition);
        }
    }

    public void HandleReset()
    {
        videoPlayer.time = 0;
        videoPlayer.Play();

        // 컷 씬 BGM 재생
        bgmSoundInstance = FMODUnity.RuntimeManager.CreateInstance("event:/BGM/CutScene");
        bgmSoundInstance.setParameterByNameWithLabel("BGM", "CutScene");
        bgmSoundInstance.start();
        bgmSoundInstance.release();

        StartCoroutine(Delay(delayInSeconds));
    }

    IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);
        TransitionManager.Instance.LoadSceneWithPlayer(newGameSceneTransition);
    }
}
