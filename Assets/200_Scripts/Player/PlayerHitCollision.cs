using UnityEngine;

public class PlayerHitCollision : MonoBehaviour
{
    // 컴포넌트
    private Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 함정 충돌
        if (collision.gameObject.CompareTag("Trap") && !GameplayManager.Instance.isGodMode)
        {
            player.Dead();
        }

        // 즉발 상호작용 충돌
        if (collision.TryGetComponent(out ICollisionable collisionable))
        {
            collisionable.OnCollision(player);

            // GravityFlip은 한 번만 충돌하도록 설정
            if (collision.TryGetComponent(out GravityFlip gravityFlip))
            {
                player.isCollidingWithGravityFlip = true;
            }
        }

        // 이동 플랫폼 충돌
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            player.currentPlatform = collision.transform;
            player.lastPlatformPosition = player.currentPlatform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 중력 반전 오브젝트에서 벗어남
        if (collision.TryGetComponent(out GravityFlip gravityFlip))
        {
            player.isCollidingWithGravityFlip = false;
        }

        // 이동 플랫폼에서 벗어남
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            player.currentPlatform = null;
        }
    }
}
