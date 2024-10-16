using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    public PlayerSpawnpoint playerSavepoint;
    public bool hasPlayerSavepoint;

    public bool isGameRunning;

    public int lastSavepointId;

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

    private int coinCount;
    public int CoinCount
    {
        get => coinCount;
        set
        {
            coinCount = value;
        }
    }

    private bool isGodMode;
    public bool IsGodMode
    {
        get => isGodMode;
        set
        {
            isGodMode = value;
            UIManager.Instance.UpdateGodModeText(value);
        }
    }

    private int flipCount;
    public int FlipCount
    {
        get => flipCount;
        set
        {
            flipCount = value;
            UIManager.Instance.UpdateFlipCountText(value);
        }
    }

    private int deathCount;
    public int DeathCount
    {
        get => deathCount;
        set
        {
            deathCount = value;
            UIManager.Instance.UpdateDeathCountText(value);
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        isGameRunning = true;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // UI 초기화
        IsGodMode = false;
        FlipCount = 0;
        DeathCount = 0;
    }

    private void Update()
    {
        if (isGameRunning)
        {
            ElapsedTime += Time.deltaTime;
        }

        // 무적모드
        if (Input.GetKeyDown(KeyCode.F1))
        {
            IsGodMode = !IsGodMode;
        }

        // 기록 초기화
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ElapsedTime = 0f;
            FlipCount = 0;
            DeathCount = 0;
        }

        // 씬 재시작
        if (Input.GetKeyDown(KeyCode.F5))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}
