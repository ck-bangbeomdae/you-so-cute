using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private SceneTransition sceneTransition;

    public void OnClickNewGameButton()
    {
        TransitionManager.Instance.LoadScene(sceneTransition);
    }

    public void OnClickContinueButton()
    {
        if (!string.IsNullOrEmpty(SettingsManager.Instance.playerProfile.PlayerSpawnpoint.sceneTransition.sceneName))
        {
            TransitionManager.Instance.LoadSceneWithPlayer(SettingsManager.Instance.playerProfile.PlayerSpawnpoint);
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
}
