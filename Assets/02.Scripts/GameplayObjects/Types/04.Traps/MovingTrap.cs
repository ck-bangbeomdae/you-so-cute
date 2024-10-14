using UnityEngine;

public class MovingTrap : MonoBehaviour
{
    // 정적 데이터
    [SerializeField] private Direction direction;
    [SerializeField] private StartDirection startDirection;
    [SerializeField] private float speed = 4f;
    [SerializeField] private LayerMask groundLayer;

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
        if (direction == Direction.Horizontal)
        {
            moveDirection = (startDirection == StartDirection.RightOrUp) ? Vector2.right : Vector2.left;
        }
        else
        {
            moveDirection = (startDirection == StartDirection.RightOrUp) ? Vector2.up : Vector2.down;
        }
    }

    private void FixedUpdate()
    {
        Vector2 newPosition = rb2d.position + moveDirection * speed * Time.fixedDeltaTime;
        rb2d.MovePosition(newPosition);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsGroundLayer(collision.gameObject.layer))
        {
            moveDirection = -moveDirection;
        }
    }

    private bool IsGroundLayer(int layer)
    {
        return (groundLayer.value & (1 << layer)) != 0;
    }

    private enum Direction
    {
        Horizontal,
        Vertical
    }

    private enum StartDirection
    {
        RightOrUp,
        LeftOrDown
    }
}
