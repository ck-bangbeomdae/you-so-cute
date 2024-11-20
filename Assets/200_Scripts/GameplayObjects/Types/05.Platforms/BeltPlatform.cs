using UnityEngine;

public class BeltPlatform : MonoBehaviour
{
    public CommonEnums.RotationDirection rotationDirection;
    public float beltSpeed = 8f;
    public float gearRotationMultiplier = 40f;

    private Transform[] gearTransforms;

    private void Start()
    {
        // "Gear"로 시작하는 자식들의 Transform을 찾아서 저장
        gearTransforms = GetComponentsInChildren<Transform>();
        gearTransforms = System.Array.FindAll(gearTransforms, t => t.name.StartsWith("Gear"));
    }

    private void Update()
    {
        // 회전 방향에 따라 회전
        float rotationSpeed = beltSpeed * gearRotationMultiplier * Time.deltaTime;
        float rotationAngle = rotationDirection == CommonEnums.RotationDirection.Clockwise ? -rotationSpeed : rotationSpeed;

        foreach (Transform gear in gearTransforms)
        {
            gear.Rotate(0f, 0f, rotationAngle);
        }
    }
}
