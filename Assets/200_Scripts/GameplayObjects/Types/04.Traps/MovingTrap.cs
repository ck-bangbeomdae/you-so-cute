using UnityEngine;

public class MovingTrap : MonoBehaviour, IResetable
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
    private Vector2 initialPosition;
    private Vector3 initialLocalScale;
    private Vector2 moveDirection;

    private void Awake()
    {
        initialPosition = transform.position;
        initialLocalScale = transform.localScale;
    }

    private void Start()
    {
        SetMoveDirection();
    }

    private void FixedUpdate()
    {
        Move();
        DetectCollision();
    }

    private void Update()
    {
        if (GameplayManager.Instance.isPaused)
        {
            return;
        }

        Rotate();
        Flip();
    }

    public void HandleReset()
    {
        SetMoveDirection();
        transform.position = initialPosition;
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
        Vector3 targetPosition = transform.position + (Vector3)moveDirection * speed * Time.fixedDeltaTime;
        transform.position = targetPosition;
    }

    private void DetectCollision()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, speed * Time.fixedDeltaTime, groundLayer);
        if (hit.collider != null)
        {
            moveDirection = -moveDirection;
        }
    }

    private void Rotate()
    {
        int directionMultiplier = rotationDirection == CommonEnums.RotationDirection.Clockwise ? -1 : 1;
        transform.Rotate(Vector3.forward, directionMultiplier * rotationSpeed);
    }

    private void Flip()
    {
        if (canFlip)
        {
            Vector3 localScale = initialLocalScale;

            if (movementDirection == CommonEnums.MovementDirection.Horizontal)
            {
                localScale.x *= Mathf.Sign(moveDirection.x);
            }
            else
            {
                localScale.y *= Mathf.Sign(moveDirection.y);
            }

            transform.localScale = localScale;
        }
    }
}
