using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // 정적 데이터
    [SerializeField] public float moveSpeed = 9f;
    [SerializeField] private float friction = 0.2f;
    [SerializeField] private float maxVerticalSpeed = 30f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 2.38f;

    // 컴포넌트
    [HideInInspector] public Rigidbody2D rb2d;
    [HideInInspector] public SkeletonAnimation skeletonAnimation;

    // FSM
    public readonly Dictionary<PlayerState, BaseState<Player>> playerStates = new Dictionary<PlayerState, BaseState<Player>>();
    public readonly StateMachine<Player> playerStateMachine = new StateMachine<Player>();
    private bool isDead;

    // 이동
    public Vector2 currentMoveDirection;
    public float currentSpeed;

    public float currentBeltSpeed;
    public float jumpPadFriction;

    private bool isGrounded;
    public bool IsGrounded
    {
        get => isGrounded;
        set
        {
            isGrounded = value;

            if (value)
            {
                isCollidingWithJumpPad = false;
            }
        }
    }

    private bool isGravityFlipped;
    public bool IsGravityFlipped
    {
        get => isGravityFlipped;
        set
        {
            isGravityFlipped = value;
            skeletonAnimation.Skeleton.ScaleY = isGravityFlipped ? -1 : 1;

            if (value)
            {
                skeletonAnimation.Skeleton.SetSkin("Red_Mag");
            }
            else
            {
                skeletonAnimation.Skeleton.SetSkin("Blue_Mag");
            }
        }
    }

    public bool isFacingLeft;
    public bool IsFacingLeft
    {
        get => isFacingLeft;
        set
        {
            isFacingLeft = value;
            skeletonAnimation.Skeleton.ScaleX = isFacingLeft ? -1 : 1;
        }
    }

    public bool isCollidingWithGravityFlip;
    public bool isCollidingWithJumpPad;

    public Transform currentPlatform;
    public Vector3 lastPlatformPosition;

    // 상호작용
    public IInteractable closestInteractable;

    // 경직 타이머
    public float stunTimer = 0f;

    private void Awake()
    {
        // 컴포넌트 초기화
        rb2d = GetComponent<Rigidbody2D>();
        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();

        // FSM 초기화
        playerStates[PlayerState.Idle] = new PlayerIdle();
        playerStates[PlayerState.Running] = new PlayerRunning();
        playerStates[PlayerState.GravityFlipping] = new PlayerGravityFlipping();
        playerStates[PlayerState.Interacting] = new PlayerInteracting();
        playerStates[PlayerState.OpeningInventory] = new PlayerOpeningInventory();
    }

    private void Start()
    {
        playerStateMachine.Setup(this, playerStates[PlayerState.Idle]);

        // 디버그 : 플레이어 스폰포인트 설정
        if (!GameplayManager.Instance.hasPlayerSavepoint)
        {
            GameplayManager.Instance.playerSavepoint.sceneTransition.sceneName = SceneManager.GetActiveScene().name;
            GameplayManager.Instance.playerSavepoint.spawnPosition = transform.position;
            GameplayManager.Instance.hasPlayerSavepoint = true;
        }
    }

    private void FixedUpdate()
    {
        // 이동 플랫폼의 위치 변화를 플레이어에게 반영
        if (currentPlatform != null)
        {
            Vector3 platformMovement = currentPlatform.position - lastPlatformPosition;
            rb2d.position += (Vector2)platformMovement;
            lastPlatformPosition = currentPlatform.position;
        }

        // 중력 방향 설정
        rb2d.gravityScale = IsGravityFlipped ? -Mathf.Abs(rb2d.gravityScale) : Mathf.Abs(rb2d.gravityScale);

        Vector2 velocity = rb2d.velocity;

        if (!isCollidingWithJumpPad)
        {
            // 이동 속도 설정
            if (currentSpeed > 0f)
            {
                velocity.x = currentMoveDirection.x * currentSpeed;
            }

            // 이동 벨트 추가 이동속도 설정
            velocity.x += currentBeltSpeed;

            // 마찰력 적용
            velocity.x = Mathf.Lerp(velocity.x, 0, friction);
        }
        else
        {
            // 마찰력 적용
            velocity.x = Mathf.Lerp(velocity.x, 0, jumpPadFriction);
        }

        // y 속도 최대치 제한
        velocity.y = Mathf.Clamp(velocity.y, -maxVerticalSpeed, maxVerticalSpeed);
        rb2d.velocity = velocity;

        // 경직 타이머 감소
        if (stunTimer > 0)
        {
            stunTimer -= Time.fixedDeltaTime;
        }
        else
        {
            // 레이캐스트를 사용하여 땅과의 충돌 감지
            IsGrounded = Physics2D.Raycast(transform.position, IsGravityFlipped ? Vector2.up : Vector2.down, groundCheckDistance, groundLayer);
        }
    }

    private void Update()
    {
        // 씬 전환 중이거나 플레이어가 사망한 경우 상태 업데이트 중지
        if (TransitionManager.Instance.isTransition || isDead || isCollidingWithJumpPad)
        {
            return;
        }

        // 플레이어 상태 업데이트
        playerStateMachine.Execute();

        // 이동 방향에 따른 좌우 반전
        if (currentMoveDirection.x < 0f)
        {
            IsFacingLeft = true;
        }
        else if (currentMoveDirection.x > 0f)
        {
            IsFacingLeft = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (CollisionUtils.IsCollisionFromTopOrBottom(collision))
        {
            if (collision.gameObject.TryGetComponent(out BeltPlatform beltPlatform))
            {
                currentBeltSpeed = beltPlatform.beltSpeed;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out BeltPlatform beltPlatform))
        {
            currentBeltSpeed = 0f;
        }
    }

    public void GravityFlip()
    {
        // 중력 반전
        GameplayManager.Instance.FlipCount++;
        IsGravityFlipped = !IsGravityFlipped;
        IsGrounded = false;

        // TODO : 중력 반전 효과음 재생
    }

    public void Dead()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        currentMoveDirection = Vector2.zero;
        currentSpeed = 0;

        // TODO : 사망 애니메이션 재생

        // TODO : 사망 효과음 재생

        GameplayManager.Instance.DeathCount++;
        TransitionManager.Instance.LoadSceneWithPlayer(GameplayManager.Instance.playerSavepoint);
    }
}

public enum PlayerState
{
    Idle,
    Running,
    GravityFlipping,
    Interacting,
    OpeningInventory
}
