using UnityEngine;

public class PlayerHitCollision : MonoBehaviour
{
    // 컴포넌트
    private Player player;
    Vector3 playPosition;

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

            if (collision.TryGetComponent(out GravityFlip gravityFlip))
            {
                // GravityFlip은 한 번만 충돌하도록 설정
                player.isCollidingWithGravityFlip = true;

                // 레이저에 의한 중력 반전 파티클 재생
                if (player.IsGravityFlipped)
                {
                    playPosition = player.transform.position;
                    playPosition.x = gravityFlip.gameObject.transform.position.x;
                    Debug.Log(playPosition);
                    Debug.Log(gravityFlip.gameObject.transform.position);
                    player.r_laserflippingParticlePrefab.transform.position = playPosition;
                    player.r_laserflippingParticlePrefab.GetComponent<ParticleSystem>().Play();
                }
                else
                {
                    playPosition = player.transform.position;
                    playPosition.x = gravityFlip.gameObject.transform.position.x;
                    player.s_laserflippingParticlePrefab.transform.position = playPosition;
                    player.s_laserflippingParticlePrefab.GetComponent<ParticleSystem>().Play();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 중력 반전 오브젝트에서 벗어남
        if (collision.TryGetComponent(out GravityFlip gravityFlip))
        {
            player.isCollidingWithGravityFlip = false;
        }
    }
}
