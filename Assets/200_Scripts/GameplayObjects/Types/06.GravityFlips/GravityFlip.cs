using UnityEngine;

public class GravityFlip : MonoBehaviour, ICollisionable
{
    public CommonEnums.MovementDirection direction;

    public void OnCollision(Player player)
    {
        if (player.isCollidingWithGravityFlip)
        {
            return;
        }

        player.rb2d.velocity /= 4f;
        player.isCollideWithGravityFlip = true;
        player.gravityFlipComboCount++;

        // 애니메이션 재생
        player.skeletonAnimation.state.SetAnimation(0, "lazer_flip", false);
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/FlipLazer");

        player.GravityFlip();
    }
}
