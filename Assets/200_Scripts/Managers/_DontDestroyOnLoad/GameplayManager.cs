using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

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


    private int currentProgressPortalCount;
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

        isGodMode = false;
        flipCount = 0;
        deathCount = 0;
    }

    private void Update()
    {
        if (IsGameRunning)
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
}
