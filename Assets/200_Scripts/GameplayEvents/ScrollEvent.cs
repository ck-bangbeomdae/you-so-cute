using Cinemachine;
using UnityEngine;

public class ScrollEvent : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.1f;
    [SerializeField] private CommonEnums.MovementDirection movementDirection;
    [SerializeField] private CommonEnums.InitialDirection initialDirection;

    private Transform virtualCameraTransform;
    private CinemachineVirtualCamera virtualCamera;
    private BoxCollider2D boundaryCollider;

    private Vector2 scrollDirection;
    private Vector2 minPosition;
    private Vector2 maxPosition;

    private void Awake()
    {
        virtualCameraTransform = transform.Find("VirtualCamera");
        virtualCamera = virtualCameraTransform.GetComponent<CinemachineVirtualCamera>();
        boundaryCollider = GetComponentInChildren<BoxCollider2D>();
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

    private void Scroll()
    {
        Vector2 newPosition = (Vector2)virtualCameraTransform.position + scrollDirection * scrollSpeed * Time.fixedDeltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, minPosition.x, maxPosition.x);
        newPosition.y = Mathf.Clamp(newPosition.y, minPosition.y, maxPosition.y);
        virtualCameraTransform.position = new Vector3(newPosition.x, newPosition.y, virtualCameraTransform.position.z);
    }

    private void CalculateBoundary()
    {
        Vector2 colliderOffset = boundaryCollider.offset;
        Vector2 colliderSize = boundaryCollider.size;

        // 가상 카메라의 크기 가져오기
        float cameraHeight = 2f * virtualCamera.m_Lens.OrthographicSize;
        float cameraWidth = cameraHeight * virtualCamera.m_Lens.Aspect;

        // 최소 위치 계산 (경계의 중앙으로 클램프)
        minPosition = (Vector2)boundaryCollider.transform.position + colliderOffset - (colliderSize / 2) + new Vector2(cameraWidth / 2, cameraHeight / 2);

        // 최대 위치 계산 (경계의 중앙으로 클램프)
        maxPosition = (Vector2)boundaryCollider.transform.position + colliderOffset + (colliderSize / 2) - new Vector2(cameraWidth / 2, cameraHeight / 2);
    }
}
