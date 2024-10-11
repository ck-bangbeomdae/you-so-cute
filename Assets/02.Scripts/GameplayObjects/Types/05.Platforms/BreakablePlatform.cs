using System.Collections;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour, IRespawnable
{
    [SerializeField] private float destructionDelay = 2.5f;

    private SpriteRenderer spriteRenderer;
    private Collider2D collider2d;

    private bool isbreaking = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider2d = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isbreaking)
        {
            if (CollisionUtils.IsCollisionFromTopOrBottom(collision))
            {
                // TODO : 플랫폼 파괴 애니메이션 재생

                // TODO : 플랫폼 파괴 효과음 재생

                StartCoroutine(DeactivateAfterDelay());
            }
        }
    }

    public void HandleRespawn()
    {
        // TODO : 플랫폼 리스폰 애니메이션 재생

        isbreaking = false;

        spriteRenderer.enabled = true;
        collider2d.enabled = true;

        StopAllCoroutines();
    }

    private IEnumerator DeactivateAfterDelay()
    {
        isbreaking = true;

        yield return new WaitForSeconds(destructionDelay);

        spriteRenderer.enabled = false;
        collider2d.enabled = false;
    }
}
