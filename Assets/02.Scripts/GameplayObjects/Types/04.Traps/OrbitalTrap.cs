using UnityEngine;

public class OrbitalTrap : MonoBehaviour
{
    // 정적 데이터
    [SerializeField] private float orbitalSpeed = 3f;
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private Direction orbitalDirection;
    [SerializeField] private Direction rotationDirection;
    [SerializeField] private float radius = 3f;

    private Vector3 centerPoint;
    private float angle;

    private void Start()
    {
        centerPoint = transform.position;
    }

    private void FixedUpdate()
    {
        Orbit();
    }

    private void Update()
    {

        Rotate();
    }

    private void Orbit()
    {
        float directionMultiplier = orbitalDirection == Direction.Clockwise ? -1 : 1;
        angle += directionMultiplier * orbitalSpeed * Time.fixedDeltaTime;
        float x = Mathf.Cos(angle) * radius;
        float y = Mathf.Sin(angle) * radius;
        transform.position = new Vector3(centerPoint.x + x, centerPoint.y + y, transform.position.z);
    }

    private void Rotate()
    {
        float directionMultiplier = rotationDirection == Direction.Clockwise ? -1 : 1;
        transform.Rotate(Vector3.forward, directionMultiplier * rotationSpeed);
    }

    private enum Direction
    {
        Clockwise,
        CounterClockwise
    }
}
