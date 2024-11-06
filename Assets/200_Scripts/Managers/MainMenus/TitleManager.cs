using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    // 컴포넌트
    [SerializeField] private SceneTransition newGameSceneTransition;
    [SerializeField] private TMP_InputField playerNameInputField;

    private void Start()
    {
        GameplayManager.Instance.isGameRunning = false;
        UpdatePlayerName();
    }

    public void OnPlayerNameChanged(string newValue)
    {
        ProfileManager.Instance.playerProfile.playerName = newValue;
        ProfileManager.Instance.SaveProfile();
    }

    public void OnClickNewGameButton()
    {
        GameplayManager.Instance.isGameRunning = true;
        TransitionManager.Instance.LoadScene(newGameSceneTransition);
    }

    public void OnClickContinueButton()
    {
        if (!string.IsNullOrEmpty(ProfileManager.Instance.playerProfile.progressSave.playerSpawnpoint.sceneTransition.sceneName))
        {
            GameplayManager.Instance.isGameRunning = true;
            TransitionManager.Instance.LoadSceneWithPlayer(ProfileManager.Instance.playerProfile.progressSave.playerSpawnpoint);
        }
    }

    public void OnClickLeaderboardButton()
    {
        SceneManager.LoadScene("Scene_Leaderboard");
    }

    public void OnClickExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    private void UpdatePlayerName()
    {
        playerNameInputField.text = ProfileManager.Instance.playerProfile.playerName;
    }
}
