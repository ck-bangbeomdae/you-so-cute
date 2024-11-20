using UnityEngine;
using UnityEngine.SceneManagement;

public class MovingPlatform : MonoBehaviour, IResetable
{
    // 정적 데이터
    public CommonEnums.MovementDirection movementDirection;
    [SerializeField] private CommonEnums.InitialDirection initialDirection;
    [SerializeField] private float speed = 4f;
    [SerializeField] private LayerMask groundLayer;

    // 컴포넌트
    private Rigidbody2D rb2d;

    // 이동
    private Vector2 initialPosition;
    private Vector2 moveDirection;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        if (gameObject.scene != SceneManager.GetActiveScene())
        {
            rb2d.simulated = false;
        }

        initialPosition = transform.position;
    }

    private void Start()
    {
        SetMoveDirection();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (CollisionUtils.IsGroundLayer(groundLayer, collision.gameObject.layer))
        {
            moveDirection = -moveDirection;
        }
    }

    public void HandleReset()
    {
        SetMoveDirection();
        transform.position = initialPosition;
        rb2d.simulated = true;
    }

    private void SetMoveDirection()
    {
        if (movementDirection == CommonEnums.MovementDirection.Horizontal)
        {
            moveDirection = (initialDirection == CommonEnums.InitialDirection.RightOrUp) ? Vector2.right : Vector2.left;
        }
        else
        {
            moveDirection = (initialDirection == CommonEnums.InitialDirection.RightOrUp) ? Vector2.up : Vector2.down;
        }
    }

    private void Move()
    {
        Vector2 newPosition = rb2d.position + moveDirection * speed * Time.fixedDeltaTime;
        rb2d.MovePosition(newPosition);
    }
}
