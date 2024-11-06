using DG.Tweening;
using UnityEngine;

public class ProximityPopOut : MonoBehaviour
{
    [SerializeField] private float popOutDistance = 1.5f;
    [SerializeField] private float animationDuration = 0.2f;
    [SerializeField] private Ease animationType = Ease.OutQuad;

    private ScrollEvent scrollEvent;
    private Transform laserTransform;

    private Vector3 originPosition;
    private Vector3 direction;

    private void Awake()
    {
        scrollEvent = GetComponentInParent<ScrollEvent>();
        laserTransform = transform.Find("Laser");
    }

    private void OnEnable()
    {
        scrollEvent.OnMaxPositionReached += HandleMaxPositionReached;
    }

    private void OnDisable()
    {
        scrollEvent.OnMaxPositionReached -= HandleMaxPositionReached;
    }

    private void Start()
    {
        originPosition = laserTransform.localPosition;

        if (scrollEvent.movementDirection == CommonEnums.MovementDirection.Horizontal)
        {
            direction = name == "RightOrUp" ? Vector3.left : Vector3.right;
        }
        else
        {
            direction = name == "LeftOrDown" ? Vector3.up : Vector3.down;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !scrollEvent.isAtMaxPosition)
        {
            Vector3 newPosition = originPosition + direction * popOutDistance;
            laserTransform.DOLocalMove(newPosition, animationDuration).SetEase(animationType);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            laserTransform.DOLocalMove(originPosition, animationDuration).SetEase(animationType);
        }
    }

    private void HandleMaxPositionReached()
    {
        laserTransform.DOLocalMove(originPosition, animationDuration).SetEase(animationType);
    }
}
