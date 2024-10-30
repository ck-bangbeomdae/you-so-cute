using DG.Tweening;
using System.Collections;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour, IRespawnable
{
    [SerializeField] private float destructionDelay = 1f;

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
                StartCoroutine(DeactivateAfterDelay());
            }
        }
    }

    public void HandleRespawn()
    {
        isbreaking = false;

        spriteRenderer.DOFade(1f, 0.3f);

        spriteRenderer.enabled = true;
        collider2d.enabled = true;

        StopAllCoroutines();
    }

    private IEnumerator DeactivateAfterDelay()
    {
        isbreaking = true;

        spriteRenderer.DOFade(0, destructionDelay).SetEase(Ease.InSine);

        yield return new WaitForSeconds(destructionDelay);

        // TODO : 플랫폼 파괴 효과음 재생

        spriteRenderer.enabled = false;
        collider2d.enabled = false;
    }
}
