using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

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

    public PlayerSpawnpoint playerSavepoint;
    public bool hasPlayerSavepoint;
    public int lastSavepointId;
    public int lastSavepointProgressPortalCount;

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

    private const int MaxProgressPortalCount = 100; // <- TODO : 최대 포탈 개수 나중에 카운트 해서 적용
    private int currentProgressPortalCount;
    public int CurrentProgressPortalCount
    {
        get => currentProgressPortalCount;
        set
        {
            currentProgressPortalCount = value;
            UIManager.Instance.UpdateProgressPortalCount(MaxProgressPortalCount, currentProgressPortalCount);
        }
    }

    public bool isGodMode;
    public int flipCount;
    public int deathCount;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        IsGameRunning = true;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // UI 초기화
        isGodMode = false;
        flipCount = 0;
        deathCount = 0;
    }

    private void Update()
    {
        if (IsGameRunning)
        {
            ElapsedTime += Time.deltaTime;
        }

        // 무적모드
        if (Input.GetKeyDown(KeyCode.F1))
        {
            isGodMode = !isGodMode;
        }

        // 세이브 포인트로 돌아가기
        if (Input.GetKeyDown(KeyCode.F5))
        {
            deathCount++;
            TransitionManager.Instance.LoadSceneWithPlayer(playerSavepoint);
        }
    }
}
