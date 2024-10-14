using UnityEngine;
using UnityEngine.SceneManagement;

public class Savepoint : BasePlayerSpawnpoint, ICollisionable
{
    // 정적 데이터
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite inactiveSprite;

    // 컴포넌트
    private SpriteRenderer spriteRenderer;

    private int id;

    private bool isActive;
    public bool IsActive
    {
        get => isActive;
        set
        {
            isActive = value;

            if (isActive)
            {
                spriteRenderer.sprite = activeSprite;
            }
            else
            {
                spriteRenderer.sprite = inactiveSprite;
            }
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnValidate()
    {
        playerSpawnpoint.sceneTransition.sceneName = SceneManager.GetActiveScene().name;
        playerSpawnpoint.spawnPosition = transform.position;
    }

    private void Start()
    {
        id = playerSpawnpoint.GetHashCode();
        IsActive = GameplayManager.Instance.lastSavepointId == id ? true : false;
    }

    public void OnCollision(Player player)
    {
        if (!IsActive)
        {
            // 세이브 파일 생성 및 덮어쓰기
            ProfileManager.Instance.playerProfile.playerSpawnpoint = playerSpawnpoint;
            ProfileManager.Instance.SaveProfile();

            // 세이브 포인트 변경
            GameplayManager.Instance.playerSavepoint = playerSpawnpoint;

            // 마지막 세이브 포인트 비활성화
            GameObject[] savepoints = GameObject.FindGameObjectsWithTag("Savepoint");
            foreach (GameObject savepoint in savepoints)
            {
                Savepoint sp = savepoint.GetComponent<Savepoint>();
                if (sp.id == GameplayManager.Instance.lastSavepointId)
                {
                    sp.IsActive = false;
                    break;
                }
            }
            GameplayManager.Instance.lastSavepointId = id;

            IsActive = true;
        }
    }
}
