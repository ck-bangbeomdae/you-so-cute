using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    // 컴포넌트
    [SerializeField] private GameObject renameModal;
    [SerializeField] private SceneTransition newGameSceneTransition;

    private TMP_InputField renameModalInputField;

    private bool isModalOpen;

    private void Awake()
    {
        renameModalInputField = renameModal.GetComponentInChildren<TMP_InputField>();
    }

    private void Start()
    {
        GameplayManager.Instance.IsGameRunning = false;
    }

    public void OnClickNewGameButton()
    {
        if (!isModalOpen)
        {
            GameplayManager.Instance.IsGameRunning = true;
            TransitionManager.Instance.LoadScene(newGameSceneTransition);
        }
    }

    public void OnClickContinueButton()
    {
        if (!isModalOpen)
        {
            if (!string.IsNullOrEmpty(ProfileManager.Instance.playerProfile.progressSave.playerSpawnpoint.sceneTransition.sceneName))
            {
                GameplayManager.Instance.IsGameRunning = true;
                GameplayManager.Instance.hasPlayerSavepoint = true;
                GameplayManager.Instance.lastSavepointId = ProfileManager.Instance.playerProfile.progressSave.lastSavepointId;
                GameplayManager.Instance.lastSavepointProgressPortalCount = ProfileManager.Instance.playerProfile.progressSave.lastSavepointProgressPortalCount;
                GameplayManager.Instance.ElapsedTime = ProfileManager.Instance.playerProfile.progressSave.elapsedTime;
                GameplayManager.Instance.CurrentProgressPortalCount = ProfileManager.Instance.playerProfile.progressSave.progressPortalCount;
                GameplayManager.Instance.flipCount = ProfileManager.Instance.playerProfile.progressSave.flipCount;
                GameplayManager.Instance.deathCount = ProfileManager.Instance.playerProfile.progressSave.deathCount;

                TransitionManager.Instance.LoadSceneWithPlayer(ProfileManager.Instance.playerProfile.progressSave.playerSpawnpoint);
            }
        }
    }

    public void OnClickLeaderboardButton()
    {
        if (!isModalOpen)
        {
            SceneManager.LoadScene("Scene_Leaderboard");
        }
    }

    public void OnClickExitButton()
    {
        if (!isModalOpen)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    public void OnClickConfirmRenameModal()
    {
        if (isModalOpen)
        {
            ProfileManager.Instance.playerProfile.playerName = renameModalInputField.text;
            ProfileManager.Instance.SaveProfile();

            renameModal.SetActive(false);

            // TODO : 블러 효과 해제

            isModalOpen = false;
        }
    }

    public void OnClickCancelRenameModal()
    {
        if (isModalOpen)
        {
            renameModal.SetActive(false);

            // TODO : 블러 효과 해제

            isModalOpen = false;
        }
    }

    private void CreateRenameModal()
    {
        if (!isModalOpen)
        {
            renameModalInputField.text = ProfileManager.Instance.playerProfile.playerName;

            renameModal.SetActive(true);

            // TODO : 블러 효과 적용

            isModalOpen = true;
        }
    }
}
