using UnityEngine;

public class JumpPadPlatform : MonoBehaviour, ICollisionable
{
    [SerializeField] private Vector2 jumpDirection = new Vector2(1, 1);
    [SerializeField] private float jumpForce = 200f;
    [SerializeField] private float jumpPadFriction = 0.04f;

    public void OnCollision(Player player)
    {
        // 기존 속도를 초기화
        player.rb2d.velocity = Vector2.zero;

        // 설정된 방향으로 힘을 가함
        player.rb2d.AddForce(jumpDirection.normalized * jumpForce, ForceMode2D.Impulse);

        // 점프 패드 충돌했을 때의 마찰력 설정
        player.jumpPadFriction = jumpPadFriction;

        // 경직 시간 설정
        player.stunTimer = 0.5f;

        player.IsGrounded = false;
        player.isCollidingWithJumpPad = true;
    }
}
