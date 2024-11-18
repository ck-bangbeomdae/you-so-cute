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

    // 이동
    private Vector2 moveDirection;

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
        DetectCollision();
    }

    private void Update()
    {
        Rotate();

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
    }

    private void Move()
    {
        Vector3 newPosition = transform.position + (Vector3)moveDirection * speed * Time.fixedDeltaTime;
        transform.position = newPosition;
    }

    private void Rotate()
    {
        float directionMultiplier = rotationDirection == CommonEnums.RotationDirection.Clockwise ? -1 : 1;
        transform.Rotate(Vector3.forward, directionMultiplier * rotationSpeed);
    }

    private void DetectCollision()
    {
        // Raycast를 사용하여 충돌 감지
        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, speed * Time.fixedDeltaTime, groundLayer);
        if (hit.collider != null)
        {
            moveDirection = -moveDirection;
        }
    }
}
