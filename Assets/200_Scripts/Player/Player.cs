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

    [SerializeField] private GameObject s_runningParticlePrefab;
    [SerializeField] private GameObject s_landingParticlePrefab;
    [SerializeField] public GameObject s_vLaserflippingParticlePrefab;
    [SerializeField] public GameObject s_hLaserflippingParticlePrefab;
    [SerializeField] private GameObject s_deadParticlePrefab;
    [SerializeField] private GameObject s_dustParticlePrefab;
    [SerializeField] private GameObject[] s_flipDustParticlePrefab = null;

    [SerializeField] private GameObject r_runningParticlePrefab;
    [SerializeField] private GameObject r_landingParticlePrefab;
    [SerializeField] public GameObject r_vLaserflippingParticlePrefab;
    [SerializeField] public GameObject r_hLaserflippingParticlePrefab;
    [SerializeField] private GameObject r_deadParticlePrefab;
    [SerializeField] private GameObject r_dustParticlePrefab;
    [SerializeField] private GameObject[] r_flipDustParticlePrefab = null;

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
    private Vector3 lastPlatformPosition;
    private MovingPlatform currentPlatform;

    public Vector2 currentMoveDirection;
    public float currentSpeed;

    public float currentBeltSpeed;
    public float jumpPadFriction;

    private bool isGrounded;
    private bool wasGrounded;

    public bool isCollideWithGravityFlip;
    public int gravityFlipComboCount;

    public bool IsGrounded
    {
        get => isGrounded;
        set
        {
            wasGrounded = isGrounded;
            isGrounded = value;

            if (isGrounded && !wasGrounded)
            {
                isCollideWithGravityFlip = false;
                isCollidingWithJumpPad = false;

                // 플레이어 착지 파티클 생성
                if (IsGravityFlipped)
                    r_landingParticlePrefab.GetComponent<ParticleSystem>().Play();
                else
                    s_landingParticlePrefab.GetComponent<ParticleSystem>().Play();

                // TODO : 착지 효과음 재생
                gravityFlipComboCount = 0;
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

    // 상호작용
    public IInteractable closestInteractable;

    // 타이머
    public float stunTimer = 0f;

    private void Awake()
    {
        if (gameObject.scene != SceneManager.GetActiveScene() && gameObject.scene.name != "Scene_정윤희_01")
        {
            Destroy(gameObject);
            return;
        }

        // 컴포넌트 초기화
        rb2d = GetComponent<Rigidbody2D>();
        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();

        GameObject darkEventObject = GameObject.FindWithTag("DarkEvent");
        if (darkEventObject != null)
        {
            darkEvent = darkEventObject.GetComponent<DarkEvent>();
            darkEvent.InitialPlayer(this);
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

    private void Start()
    {
        playerStateMachine.Setup(this, playerStates[PlayerState.Idle]);

        // 플레이어 스폰포인트 설정
        if (!GameplayManager.Instance.hasPlayerSavepoint)
        {
            GameplayManager.Instance.playerSavepoint.sceneTransition.sceneName = gameObject.scene.name;
            GameplayManager.Instance.playerSavepoint.spawnPosition = transform.position;
            GameplayManager.Instance.hasPlayerSavepoint = true;
        }
    }

    private void FixedUpdate()
    {
        // 발판의 이동량을 계산하여 플레이어의 위치를 부드럽게 업데이트
        if (currentPlatform != null)
        {
            Vector3 platformMovement = currentPlatform.transform.position - lastPlatformPosition;
            Vector3 targetPosition = rb2d.position + (Vector2)platformMovement;
            rb2d.position = Vector3.Lerp(rb2d.position, targetPosition, currentPlatform.movementDirection == CommonEnums.MovementDirection.Horizontal ? 1f : 0.1f);
            lastPlatformPosition = currentPlatform.transform.position;
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

        // 플레이어 눈 변경
        var eyeSlot = skeletonAnimation.Skeleton.FindSlot("eye");
        if (isCollideWithGravityFlip)
        {
            if (gravityFlipComboCount <= 1)
            {
                eyeSlot.Attachment = skeletonAnimation.Skeleton.GetAttachment("eye", "eye_flip_1");
            }
            else if (gravityFlipComboCount <= 2)
            {
                eyeSlot.Attachment = skeletonAnimation.Skeleton.GetAttachment("eye", "eye_flip_2");
            }
            else
            {
                eyeSlot.Attachment = skeletonAnimation.Skeleton.GetAttachment("eye", "eye_flip_3");
            }
        }
        else
        {
            eyeSlot.Attachment = skeletonAnimation.Skeleton.GetAttachment("eye", "eye");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform") && CollisionUtils.IsCollisionFromTopOrBottom(collision))
        {
            currentPlatform = collision.transform.GetComponent<MovingPlatform>();
            lastPlatformPosition = currentPlatform.transform.position;
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
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            // 발판에서 떨어지면 현재 발판을 초기화
            currentPlatform = null;
        }

        if (collision.gameObject.TryGetComponent(out BeltPlatform beltPlatform))
        {
            currentBeltSpeed = 0f;
        }
    }

    private void HandleAnimationEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "run")
        {
            // 달리기 파티클 재생
            if (IsGravityFlipped)
                r_runningParticlePrefab.GetComponent<ParticleSystem>().Play();
            else
                s_runningParticlePrefab.GetComponent<ParticleSystem>().Play();

            // TODO : 달리기 효과음 재생
        }
    }

    public void GravityFlip()
    {
        if (isDead)
        {
            return;
        }

        // 중력 반전
        GameplayManager.Instance.flipCount++;
        IsGravityFlipped = !IsGravityFlipped;
        IsGrounded = false;

        // 중력 반전시 바닥 먼지 파티클 재생
        if (r_dustParticlePrefab != null && s_dustParticlePrefab != null)
        {
            if (IsGravityFlipped)
            {
                s_dustParticlePrefab.GetComponent<ParticleSystem>().Stop();
                r_dustParticlePrefab.GetComponent<ParticleSystem>().Play();

                foreach (GameObject p in r_flipDustParticlePrefab)
                {
                    p.GetComponent<ParticleSystem>().Play();
                }
            }
            else
            {
                r_dustParticlePrefab.GetComponent<ParticleSystem>().Stop();
                s_dustParticlePrefab.GetComponent<ParticleSystem>().Play();

                foreach (GameObject p in s_flipDustParticlePrefab)
                {
                    p.GetComponent<ParticleSystem>().Play();
                }
            }
        }

        // TODO : 중력 반전 효과음 재생
    }

    public void Dead()
    {
        if (isDead)
        {
            return;
        }

        GameplayManager.Instance.deathCount++;

        // 물리 시뮬레이션 정지
        rb2d.simulated = false;

        // 사망 애니메이션 재생
        var trackEntry = skeletonAnimation.state.SetAnimation(0, "death", false);
        trackEntry.Complete += (entry) =>
        {
            // 플레이어 리스폰
            TransitionManager.Instance.LoadSceneWithPlayer(GameplayManager.Instance.playerSavepoint);

            // 진행사항 되돌리기
            GameplayManager.Instance.CurrentProgressPortalCount = GameplayManager.Instance.lastSavepointProgressPortalCount;
        };

        // 사망 파티클 재생
        if (IsGravityFlipped)
        {
            r_deadParticlePrefab.GetComponent<ParticleSystem>().Play();
        }
        else
        {
            s_deadParticlePrefab.GetComponent<ParticleSystem>().Play();
        }

        // TODO : 사망 효과음 재생

        if (scrollEvent != null)
        {
            scrollEvent.MoveToSavepoint();
        }

        isDead = true;
    }
}

public enum PlayerState
{
    Idle,
    InAir,
    Running,
    GravityFlipping,
    Interacting
}
