using UnityEngine;
using UnityEngine.UI;

public class OptionsModalUI : MonoBehaviour
{
    [SerializeField] private Slider optionsModalBGMSlider;
    [SerializeField] private Slider optionsModalSFXSlider;

    private void OnEnable()
    {
        optionsModalBGMSlider.value = ProfileManager.Instance.playerProfile.bgmVolume;
        optionsModalSFXSlider.value = ProfileManager.Instance.playerProfile.sfxVolume;
    }

    public void OnClickExitButton()
    {
        Save();

        // 진행사항 초기화
        GameplayManager.Instance.IsGameRunning = false;
        GameplayManager.Instance.ResetProgress();

        UIManager.Instance.CloseManual();

        SceneTransition titleSceneTransition = new SceneTransition { sceneName = "Scene_Title", transitionType = TransitionType.FadeInOut };
        TransitionManager.Instance.LoadScene(titleSceneTransition);
    }

    public void OnClickResumeButton()
    {
        Save();
    }

    private void Save()
    {
        ProfileManager.Instance.playerProfile.bgmVolume = optionsModalBGMSlider.value;
        ProfileManager.Instance.playerProfile.sfxVolume = optionsModalSFXSlider.value;
        ProfileManager.Instance.SaveProfile();
        GameplayManager.Instance.TogglePause();
    }
}
