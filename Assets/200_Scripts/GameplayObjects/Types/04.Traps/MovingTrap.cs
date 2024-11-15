using UnityEngine;

public class MovingTrap : MonoBehaviour
{
    // 정적 데이터
    [SerializeField] private float speed = 8f;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private CommonEnums.MovementDirection movementDirection;
    [SerializeField] private CommonEnums.InitialDirection initialDirection;
    [SerializeField] private CommonEnums.RotationDirection rotationDirection;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool canFlip = true;

    // 컴포넌트
    private Rigidbody2D rb2d;

    // 이동
    private Vector2 moveDirection;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // 시작 방향 설정
        if (movementDirection == CommonEnums.MovementDirection.Horizontal)
        {
            moveDirection = (initialDirection == CommonEnums.InitialDirection.RightOrUp) ? Vector2.right : Vector2.left;
        }
        else
        {
            moveDirection = (initialDirection == CommonEnums.InitialDirection.RightOrUp) ? Vector2.up : Vector2.down;
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {
        if (canFlip)
        {
            Vector3 localScale = transform.localScale;

            if (movementDirection == CommonEnums.MovementDirection.Horizontal)
            {
                localScale.x = moveDirection.x;
            }
            else
            {
                localScale.y = moveDirection.y;
            }

            transform.localScale = localScale;
        }

        Rotate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CollisionUtils.IsGroundLayer(groundLayer, collision.gameObject.layer))
        {
            moveDirection = -moveDirection;
        }
    }

    private void Move()
    {
        Vector2 newPosition = rb2d.position + moveDirection * speed * Time.fixedDeltaTime;
        rb2d.MovePosition(newPosition);
    }

    private void Rotate()
    {
        float directionMultiplier = rotationDirection == CommonEnums.RotationDirection.Clockwise ? -1 : 1;
        transform.Rotate(Vector3.forward, directionMultiplier * rotationSpeed);
    }
}
