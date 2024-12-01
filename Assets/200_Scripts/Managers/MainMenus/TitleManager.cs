using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour, IResetable
{
    // 컴포넌트
    [SerializeField] private GameObject renameModal;
    [SerializeField] private GameObject optionsModal;

    [SerializeField] private GameObject newGameSelect;
    [SerializeField] private GameObject continueSelect;
    [SerializeField] private GameObject leaderboardSelect;
    [SerializeField] private GameObject optionsSelect;
    [SerializeField] private GameObject exitSelect;

    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button leaderboardButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;

    [SerializeField] private TMP_InputField renameModalInputField;
    [SerializeField] private Slider optionsModalBGMSlider;
    [SerializeField] private Slider optionsModalSFXSlider;

    private Vector2 newGameTargetTransform;
    private Vector2 continueTargetTransform;
    private Vector2 leaderboardTargetTransform;
    private Vector2 optionsTargetTransform;
    private Vector2 exitTargetTransform;

    private bool isModalOpen;

    private void Awake()
    {
        newGameTargetTransform = newGameSelect.transform.position;
        continueTargetTransform = continueSelect.transform.position;
        leaderboardTargetTransform = leaderboardSelect.transform.position;
        optionsTargetTransform = optionsSelect.transform.position;
        exitTargetTransform = exitSelect.transform.position;
    }

    private void Start()
    {
        HandleReset();
    }

    public void HandleReset()
    {
        newGameSelect.transform.position = new Vector2(2200f, newGameSelect.transform.position.y);
        continueSelect.transform.position = new Vector2(2200f, continueSelect.transform.position.y);
        leaderboardSelect.transform.position = new Vector2(2200f, leaderboardSelect.transform.position.y);
        optionsSelect.transform.position = new Vector2(2200f, optionsSelect.transform.position.y);
        exitSelect.transform.position = new Vector2(2200f, exitSelect.transform.position.y);

        newGameButton.image.color = new Color(0.72f, 0.72f, 0.73f);
        continueButton.image.color = new Color(0.72f, 0.72f, 0.73f);
        leaderboardButton.image.color = new Color(0.72f, 0.72f, 0.73f);
        optionsButton.image.color = new Color(0.72f, 0.72f, 0.73f);
        exitButton.image.color = new Color(0.72f, 0.72f, 0.73f);
    }

    public void OnClickNewGameButton()
    {
        if (!TransitionManager.Instance.isTransition && !isModalOpen)
        {
            CreateRenameModal();
        }
    }

    public void OnClickContinueButton()
    {
        if (!TransitionManager.Instance.isTransition && !isModalOpen)
        {
            if (!string.IsNullOrEmpty(ProfileManager.Instance.playerProfile.progressSave.playerSpawnpoint.sceneTransition.sceneName))
            {
                GameplayManager.Instance.IsGameRunning = true;
                GameplayManager.Instance.playerSavepoint = ProfileManager.Instance.playerProfile.progressSave.playerSpawnpoint;
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
        if (!TransitionManager.Instance.isTransition && !isModalOpen)
        {
            SceneTransition leaderboardSceneTransition = new SceneTransition { sceneName = "Scene_Leaderboard", transitionType = TransitionType.FadeInOut };
            TransitionManager.Instance.LoadScene(leaderboardSceneTransition);
        }
    }

    public void OnClickOptionButton()
    {
        if (!TransitionManager.Instance.isTransition && !isModalOpen)
        {
            optionsModalBGMSlider.value = ProfileManager.Instance.playerProfile.bgmVolume;
            optionsModalSFXSlider.value = ProfileManager.Instance.playerProfile.sfxVolume;

            optionsModal.SetActive(true);

            // TODO : 블러 효과 적용

            newGameSelect.transform.DOMoveX(2200f, 0.5f).SetEase(Ease.OutBack);
            newGameButton.image.DOColor(new Color(0.72f, 0.72f, 0.73f, 1f), 0.5f).SetEase(Ease.InOutQuad);

            continueSelect.transform.DOMoveX(2200f, 0.5f).SetEase(Ease.OutBack);
            continueButton.image.DOColor(new Color(0.72f, 0.72f, 0.73f, 1f), 0.5f).SetEase(Ease.InOutQuad);

            leaderboardSelect.transform.DOMoveX(2200f, 0.5f).SetEase(Ease.OutBack);
            leaderboardButton.image.DOColor(new Color(0.72f, 0.72f, 0.73f, 1f), 0.5f).SetEase(Ease.InOutQuad);

            optionsSelect.transform.DOMoveX(2200f, 0.5f).SetEase(Ease.OutBack);
            optionsButton.image.DOColor(new Color(0.72f, 0.72f, 0.73f, 1f), 0.5f).SetEase(Ease.InOutQuad);

            exitSelect.transform.DOMoveX(2200f, 0.5f).SetEase(Ease.OutBack);
            exitButton.image.DOColor(new Color(0.72f, 0.72f, 0.73f, 1f), 0.5f).SetEase(Ease.InOutQuad);

            isModalOpen = true;
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

            SceneTransition sceneTransition = new SceneTransition { sceneName = "Scene_CutScene", transitionType = TransitionType.FadeInOut };
            TransitionManager.Instance.LoadScene(sceneTransition);

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

    public void OnClickConfirmOptionsModal()
    {
        if (isModalOpen)
        {
            optionsModal.SetActive(false);

            // TODO : 블러 효과 해제

            isModalOpen = false;
        }
    }

    public void OnChangedSFXVolume()
    {
        ProfileManager.Instance.playerProfile.sfxVolume = optionsModalSFXSlider.value;
        ProfileManager.Instance.SaveProfile();
    }

    public void OnChangedBGMVolume()
    {
        ProfileManager.Instance.playerProfile.bgmVolume = optionsModalBGMSlider.value;
        ProfileManager.Instance.SaveProfile();
    }

    public void CreateRenameModal()
    {
        if (!isModalOpen)
        {
            renameModalInputField.text = ProfileManager.Instance.playerProfile.playerName;

            renameModal.SetActive(true);

            // TODO : 블러 효과 적용

            newGameSelect.transform.DOMoveX(2200f, 0.5f).SetEase(Ease.OutBack);
            newGameButton.image.DOColor(new Color(0.72f, 0.72f, 0.73f, 1f), 0.5f).SetEase(Ease.InOutQuad);

            continueSelect.transform.DOMoveX(2200f, 0.5f).SetEase(Ease.OutBack);
            continueButton.image.DOColor(new Color(0.72f, 0.72f, 0.73f, 1f), 0.5f).SetEase(Ease.InOutQuad);

            leaderboardSelect.transform.DOMoveX(2200f, 0.5f).SetEase(Ease.OutBack);
            leaderboardButton.image.DOColor(new Color(0.72f, 0.72f, 0.73f, 1f), 0.5f).SetEase(Ease.InOutQuad);

            optionsSelect.transform.DOMoveX(2200f, 0.5f).SetEase(Ease.OutBack);
            optionsButton.image.DOColor(new Color(0.72f, 0.72f, 0.73f, 1f), 0.5f).SetEase(Ease.InOutQuad);

            exitSelect.transform.DOMoveX(2200f, 0.5f).SetEase(Ease.OutBack);
            exitButton.image.DOColor(new Color(0.72f, 0.72f, 0.73f, 1f), 0.5f).SetEase(Ease.InOutQuad);

            isModalOpen = true;
        }
    }

    #region 마우스 이벤트
    public void OnMouseEnterNewGameButton()
    {
        if (!TransitionManager.Instance.isTransition && !isModalOpen)
        {
            newGameSelect.transform.DOMoveX(newGameTargetTransform.x, 0.5f).SetEase(Ease.OutBack);
            newGameButton.image.DOColor(new Color(0f, 0f, 0f, 1f), 0.5f).SetEase(Ease.InOutQuad);
        }
    }

    public void OnMouseOverNewGameButton()
    {
        if (!TransitionManager.Instance.isTransition && !isModalOpen)
        {
            newGameSelect.transform.DOMoveX(2200f, 0.5f).SetEase(Ease.OutBack);
            newGameButton.image.DOColor(new Color(0.72f, 0.72f, 0.73f, 1f), 0.5f).SetEase(Ease.InOutQuad);
        }
    }

    public void OnMouseEnterContinueButton()
    {
        if (!TransitionManager.Instance.isTransition && !isModalOpen)
        {
            continueSelect.transform.DOMoveX(continueTargetTransform.x, 0.5f).SetEase(Ease.OutBack);
            continueButton.image.DOColor(new Color(0f, 0f, 0f, 1f), 0.5f).SetEase(Ease.InOutQuad);
        }
    }

    public void OnMouseOverContinueButton()
    {
        if (!TransitionManager.Instance.isTransition && !isModalOpen)
        {
            continueSelect.transform.DOMoveX(2200f, 0.5f).SetEase(Ease.OutBack);
            continueButton.image.DOColor(new Color(0.72f, 0.72f, 0.73f, 1f), 0.5f).SetEase(Ease.InOutQuad);
        }
    }

    public void OnMouseEnterLeaderboardButton()
    {
        if (!TransitionManager.Instance.isTransition && !isModalOpen)
        {
            leaderboardSelect.transform.DOMoveX(leaderboardTargetTransform.x, 0.5f).SetEase(Ease.OutBack);
            leaderboardButton.image.DOColor(new Color(0f, 0f, 0f, 1f), 0.5f).SetEase(Ease.InOutQuad);
        }
    }

    public void OnMouseOverLeaderboardButton()
    {
        if (!TransitionManager.Instance.isTransition && !isModalOpen)
        {
            leaderboardSelect.transform.DOMoveX(2200f, 0.5f).SetEase(Ease.OutBack);
            leaderboardButton.image.DOColor(new Color(0.72f, 0.72f, 0.73f, 1f), 0.5f).SetEase(Ease.InOutQuad);
        }
    }

    public void OnMouseEnterOptionsButton()
    {
        if (!TransitionManager.Instance.isTransition && !isModalOpen)
        {
            optionsSelect.transform.DOMoveX(optionsTargetTransform.x, 0.5f).SetEase(Ease.OutBack);
            optionsButton.image.DOColor(new Color(0f, 0f, 0f, 1f), 0.5f).SetEase(Ease.InOutQuad);
        }
    }

    public void OnMouseOverOptionsGameButton()
    {
        if (!TransitionManager.Instance.isTransition && !isModalOpen)
        {
            optionsSelect.transform.DOMoveX(2200f, 0.5f).SetEase(Ease.OutBack);
            optionsButton.image.DOColor(new Color(0.72f, 0.72f, 0.73f, 1f), 0.5f).SetEase(Ease.InOutQuad);
        }
    }

    public void OnMouseEnterExitButton()
    {
        if (!TransitionManager.Instance.isTransition && !isModalOpen)
        {
            exitSelect.transform.DOMoveX(exitTargetTransform.x, 0.5f).SetEase(Ease.OutBack);
            exitButton.image.DOColor(new Color(0f, 0f, 0f, 1f), 0.5f).SetEase(Ease.InOutQuad);
        }
    }

    public void OnMouseOverExitButton()
    {
        if (!TransitionManager.Instance.isTransition && !isModalOpen)
        {
            exitSelect.transform.DOMoveX(2200f, 0.5f).SetEase(Ease.OutBack);
            exitButton.image.DOColor(new Color(0.72f, 0.72f, 0.73f, 1f), 0.5f).SetEase(Ease.InOutQuad);
        }
    }
    #endregion
}
