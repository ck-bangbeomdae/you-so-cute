using UnityEngine;

public class OrbitalTrap : MonoBehaviour, IResetable
{
    // 정적 데이터
    [SerializeField] private float orbitalSpeed = 3f;
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private CommonEnums.RotationDirection orbitalDirection;
    [SerializeField] private CommonEnums.RotationDirection rotationDirection;
    [SerializeField] private float radius = 3f;

    // 이동
    private Vector2 initialPosition;
    private float angle;

    private void Awake()
    {
        initialPosition = transform.position;
    }

    private void FixedUpdate()
    {
        Orbit();
    }

    private void Update()
    {
        if (GameplayManager.Instance.isPaused)
        {
            return;
        }

        Rotate();
    }

    public void HandleReset()
    {
        transform.position = initialPosition;
        angle = 0f;
    }

    private void Orbit()
    {
        int directionMultiplier = orbitalDirection == CommonEnums.RotationDirection.Clockwise ? -1 : 1;
        angle += directionMultiplier * orbitalSpeed * Time.fixedDeltaTime;
        float x = Mathf.Cos(angle) * radius;
        float y = Mathf.Sin(angle) * radius;
        transform.position = new Vector3(initialPosition.x + x, initialPosition.y + y, transform.position.z);
    }

    private void Rotate()
    {
        int directionMultiplier = rotationDirection == CommonEnums.RotationDirection.Clockwise ? -1 : 1;
        transform.Rotate(Vector3.forward, directionMultiplier * rotationSpeed);
    }
}
