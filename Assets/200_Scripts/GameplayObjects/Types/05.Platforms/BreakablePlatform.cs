using DG.Tweening;
using System.Collections;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour, IResetable
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
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/BreakBlock");

                StartCoroutine(DeactivateAfterDelay());
            }
        }
    }

    public void HandleReset()
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

        spriteRenderer.enabled = false;
        collider2d.enabled = false;
    }
}
