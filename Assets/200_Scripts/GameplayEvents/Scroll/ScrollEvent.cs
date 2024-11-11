using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class ScrollEvent : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 1.5f;
    [SerializeField] private float deathScrollDistance = 5f;
    [SerializeField] private float deathScrollanimationDuration = 0.5f;
    [SerializeField] private Ease deathScrollanimationType = Ease.InOutSine;
    [SerializeField] private BoxCollider2D boundaryCollider;
    public CommonEnums.MovementDirection movementDirection;
    public CommonEnums.InitialDirection initialDirection;

    private Transform scrollableObject;
    private CinemachineVirtualCamera virtualCamera;

    public bool isAtMaxPosition;
    private Vector2 scrollDirection;
    private Vector2 minPosition;
    private Vector2 maxPosition;

    public delegate void MaxPositionReachedHandler();
    public event MaxPositionReachedHandler OnMaxPositionReached;

    private void Awake()
    {
        scrollableObject = transform.Find("Scrollable");
        virtualCamera = scrollableObject.GetComponentInChildren<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        // 스크롤 방향 설정
        scrollDirection = (movementDirection == CommonEnums.MovementDirection.Horizontal) ? Vector2.right : Vector2.up;
        scrollDirection *= (initialDirection == CommonEnums.InitialDirection.RightOrUp) ? 1 : -1;

        // 최소 및 최대 위치 계산
        CalculateBoundary();
    }

    private void FixedUpdate()
    {
        Scroll();
    }

    public void MoveToSavepoint()
    {
        Vector3 spawnPosition = GameplayManager.Instance.playerSavepoint.spawnPosition;
        Vector3 newPosition = scrollableObject.position;

        if (movementDirection == CommonEnums.MovementDirection.Horizontal)
        {
            newPosition.x = spawnPosition.x + (initialDirection == CommonEnums.InitialDirection.RightOrUp ? deathScrollDistance : -deathScrollDistance);
        }
        else
        {
            newPosition.y = spawnPosition.y + (initialDirection == CommonEnums.InitialDirection.RightOrUp ? deathScrollDistance : -deathScrollDistance);
        }

        scrollableObject.DOMove(newPosition, deathScrollanimationDuration).SetEase(deathScrollanimationType);
    }

    private void Scroll()
    {
        Vector2 newPosition = (Vector2)scrollableObject.position + scrollDirection * scrollSpeed * Time.fixedDeltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, minPosition.x, maxPosition.x);
        newPosition.y = Mathf.Clamp(newPosition.y, minPosition.y, maxPosition.y);

        // 최대 위치에 도달했는지 확인
        bool reachedMaxPosition = (newPosition.x == maxPosition.x || newPosition.y == maxPosition.y);
        if (reachedMaxPosition && !isAtMaxPosition)
        {
            isAtMaxPosition = true;
            OnMaxPositionReached?.Invoke();
        }

        scrollableObject.position = new Vector3(newPosition.x, newPosition.y, scrollableObject.position.z);
    }

    private void CalculateBoundary()
    {
        Vector2 colliderOffset = boundaryCollider.offset;
        Vector2 colliderSize = boundaryCollider.size;

        float cameraHeight = 2f * virtualCamera.m_Lens.OrthographicSize;
        float cameraWidth = cameraHeight * virtualCamera.m_Lens.Aspect;

        minPosition = (Vector2)boundaryCollider.transform.position + colliderOffset - (colliderSize / 2) + new Vector2(cameraWidth / 2, cameraHeight / 2);
        maxPosition = (Vector2)boundaryCollider.transform.position + colliderOffset + (colliderSize / 2) - new Vector2(cameraWidth / 2, cameraHeight / 2);
    }
}
