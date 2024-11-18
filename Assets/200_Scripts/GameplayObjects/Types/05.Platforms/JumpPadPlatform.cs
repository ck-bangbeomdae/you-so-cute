using Spine.Unity;
using UnityEngine;

public class JumpPadPlatform : MonoBehaviour, ICollisionable
{
    [SerializeField] private JumpPadType jumpPadType;

    [SerializeField] private Vector2 jumpDirection = new Vector2(1f, 1f);
    [SerializeField] private float jumpForce = 24f;
    [SerializeField] private float jumpPadFriction = 0.04f;

    private SkeletonAnimation skeletonAnimation;

    private void Awake()
    {
        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
        skeletonAnimation.timeScale = 0f;
    }

    public void OnCollision(Player player)
    {
        // 기존 속도를 초기화
        player.rb2d.velocity = Vector2.zero;

        // 설정된 방향으로 힘을 가함
        player.rb2d.AddForce(jumpDirection.normalized * jumpForce, ForceMode2D.Impulse);

        // 점프 패드 충돌했을 때의 마찰력 설정
        player.jumpPadFriction = jumpPadFriction;

        player.IsGrounded = false;
        player.isCollidingWithJumpPad = true;

        // 경직 시간 설정
        player.stunTimer = 0.5f;

        // 애니메이션 재생 속도를 1로 설정
        skeletonAnimation.timeScale = 1f;

        // 점프패드 애니메이션 재생
        skeletonAnimation.state.SetAnimation(0, jumpPadType == JumpPadType.Low ? "Jump_Pad_2" : "Jump_Pad_1", false);
    }

    private enum JumpPadType
    {
        Low,
        High
    }
}
