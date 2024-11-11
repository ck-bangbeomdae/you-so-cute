using Spine;
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

    [SerializeField] private Transform pivotReverseGround;
    [SerializeField] private Transform pivotStraightGround;
    [SerializeField] private GameObject s_runningParticlePrefab;
    [SerializeField] private GameObject s_landingParticlePrefab;
    [SerializeField] private GameObject s_deadParticlePrefab;
    [SerializeField] private GameObject r_runningParticlePrefab;
    [SerializeField] private GameObject r_landingParticlePrefab;
    [SerializeField] private GameObject r_deadParticlePrefab;

    // 컴포넌트
    [HideInInspector] public Rigidbody2D rb2d;
    [HideInInspector] public SkeletonAnimation skeletonAnimation;
    private DarkEvent darkEvent;
    private ScrollEvent scrollEvent;

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
    private bool wasGrounded;

    public bool IsGrounded
    {
        get => isGrounded;
        set
        {
            wasGrounded = isGrounded;
            isGrounded = value;

            if (isGrounded && !wasGrounded)
            {
                isCollidingWithJumpPad = false;

                // 플레이어 착지 파티클 생성
                /*GameObject landingParticleObject = Instantiate(landingParticlePrefab, isGravityFlipped ? pivotReverseGround.position : pivotStraightGround.position, Quaternion.identity);
                Vector3 landingParticleScale = landingParticleObject.transform.localScale;
                landingParticleObject.transform.localScale = new Vector3(landingParticleScale.x, isGravityFlipped ? -landingParticleScale.y : landingParticleScale.y, 1);*/
                if (IsGravityFlipped)
                    r_landingParticlePrefab.GetComponent<ParticleSystem>().Play();
                else
                    s_landingParticlePrefab.GetComponent<ParticleSystem>().Play();

                // TODO : 착지 효과음 재생
            }
        }
    }

    private bool isGravityFlipped;
    public bool IsGravityFlipped
    {
        get => isGravityFlipped;
        set
        {
            if (darkEvent != null)
            {
                darkEvent.TriggerBright();
            }

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

    // 타이머
    public float stunTimer = 0f;

    private void Awake()
    {
        // 컴포넌트 초기화
        rb2d = GetComponent<Rigidbody2D>();
        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();

        GameObject darkEventObject = GameObject.FindWithTag("DarkEvent");
        if (darkEventObject != null)
        {
            darkEvent = darkEventObject.GetComponent<DarkEvent>();
            darkEvent.player = this;
        }

        GameObject scrollEventObject = GameObject.FindWithTag("ScrollEvent");
        if (scrollEventObject != null)
        {
            scrollEvent = scrollEventObject.GetComponent<ScrollEvent>();
        }

        // FSM 초기화
        playerStates[PlayerState.Idle] = new PlayerIdle();
        playerStates[PlayerState.InAir] = new PlayerInAir();
        playerStates[PlayerState.Running] = new PlayerRunning();
        playerStates[PlayerState.GravityFlipping] = new PlayerGravityFlipping();
        playerStates[PlayerState.Interacting] = new PlayerInteracting();

        // Spine 애니메이션 이벤트 리스너 추가
        skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
    }

    private void HandleAnimationEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "run")
        {
            // 달리기 파티클 재생
            /*GameObject runningParticleObject = Instantiate(runningParticlePrefab, isGravityFlipped ? pivotReverseGround.position : pivotStraightGround.position, Quaternion.identity);
            Vector3 landingParticleScale = runningParticleObject.transform.localScale;
            runningParticleObject.transform.localScale = new Vector3(landingParticleScale.x, isGravityFlipped ? -landingParticleScale.y : landingParticleScale.y, 1);*/
            if (IsGravityFlipped)
                r_runningParticlePrefab.GetComponent<ParticleSystem>().Play();
            else
                s_runningParticlePrefab.GetComponent<ParticleSystem>().Play();

            // TODO : 달리기 효과음 재생
        }
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
            velocity.x = currentMoveDirection.x * currentSpeed;

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
        if (TransitionManager.Instance.isTransition || isDead)
        {
            return;
        }

        // 플레이어 상태 업데이트
        playerStateMachine.Execute();

        // 이동 방향에 따른 좌우 반전
        if (!isCollidingWithJumpPad)
        {
            if (currentMoveDirection.x < 0f)
            {
                IsFacingLeft = true;
            }
            else if (currentMoveDirection.x > 0f)
            {
                IsFacingLeft = false;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out BeltPlatform beltPlatform))
        {
            bool isCollisionFromTop = CollisionUtils.IsCollisionFromTop(collision);
            bool isClockwise = beltPlatform.rotationDirection == CommonEnums.RotationDirection.Clockwise;

            if (isCollisionFromTop)
            {
                currentBeltSpeed = isClockwise ? beltPlatform.beltSpeed : -beltPlatform.beltSpeed;
            }
            else
            {
                currentBeltSpeed = isClockwise ? -beltPlatform.beltSpeed : beltPlatform.beltSpeed;
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

        // 애니메이션 재생
        skeletonAnimation.state.SetAnimation(0, "flipping", false);

        // TODO : 중력 반전 효과음 재생
    }

    public void Dead()
    {
        if (isDead)
        {
            return;
        }

        currentMoveDirection = Vector2.zero;
        currentSpeed = 0;

        // TODO : 사망 애니메이션 재생
        if (IsGravityFlipped)
            r_deadParticlePrefab.GetComponent<ParticleSystem>().Play();
        else
            s_deadParticlePrefab.GetComponent<ParticleSystem>().Play();


        // TODO : 사망 효과음 재생

        if (scrollEvent != null)
        {
            scrollEvent.MoveToSavepoint();
        }

        GameplayManager.Instance.DeathCount++;
        TransitionManager.Instance.LoadSceneWithPlayer(GameplayManager.Instance.playerSavepoint);

        isDead = true;
    }
}

public enum PlayerState
{
    Idle,
    InAir,
    Running,
    GravityFlipping,
    Interacting,
}
