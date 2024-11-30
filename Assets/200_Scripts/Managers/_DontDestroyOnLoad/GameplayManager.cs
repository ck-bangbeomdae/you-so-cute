using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    [SerializeField] private GameObject optionsModal;

    public bool isPaused;
    public int MaxProgressPortalCount = 50;
    public bool isGodMode;

    private bool isGameRunning;
    public bool IsGameRunning
    {
        get => isGameRunning;
        set
        {
            isGameRunning = value;
            UIManager.Instance.ToggleGameplayUI(value);
        }
    }

    [HideInInspector] public PlayerSpawnpoint playerSavepoint;
    [HideInInspector] public bool hasPlayerSavepoint;
    [HideInInspector] public int lastSavepointId;
    [HideInInspector] public int lastSavepointProgressPortalCount;

    private float elapsedTime = 0f;
    public float ElapsedTime
    {
        get => elapsedTime;
        set
        {
            elapsedTime = value;
            UIManager.Instance.UpdateElapsedTime(StringUtils.FormatElapsedTime(value));
        }
    }

    public int currentProgressPortalCount;
    public int CurrentProgressPortalCount
    {
        get => currentProgressPortalCount;
        set
        {
            bool isDecreasing = value < currentProgressPortalCount;
            currentProgressPortalCount = value;
            UIManager.Instance.UpdateProgressPortalCount(MaxProgressPortalCount, currentProgressPortalCount, isDecreasing);
        }
    }

    [HideInInspector] public int flipCount;
    [HideInInspector] public int deathCount;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // UI 초기화
        IsGameRunning = (SceneManager.GetActiveScene().name == "Scene_Title" || SceneManager.GetActiveScene().name == "Scene_Leaderboard") ? false : true;
        CurrentProgressPortalCount = 0;
    }

    private void Update()
    {
        if (!IsGameRunning)
        {
            return;
        }

        // 게임 일시정지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (!isPaused)
        {
            ElapsedTime += Time.deltaTime;

            // 무적모드
            if (Input.GetKeyDown(KeyCode.F1))
            {
                //isGodMode = !isGodMode;
            }

            // 세이브 포인트로 돌아가기
            if (Input.GetKeyDown(KeyCode.F5))
            {
                deathCount++;
                TransitionManager.Instance.LoadSceneWithPlayer(playerSavepoint);
            }
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        optionsModal.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ResetProgress()
    {
        playerSavepoint = new PlayerSpawnpoint();
        hasPlayerSavepoint = false;
        lastSavepointId = 0;
        lastSavepointProgressPortalCount = 0;
        ElapsedTime = 0f;
        CurrentProgressPortalCount = 0;
        flipCount = 0;
        deathCount = 0;
    }
}
