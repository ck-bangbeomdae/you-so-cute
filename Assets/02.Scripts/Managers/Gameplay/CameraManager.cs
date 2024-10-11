using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    private CinemachineCamera cinemachineCamera;

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;

        cinemachineCamera = GetComponentInChildren<CinemachineCamera>();
    }

    public void SetFollowTarget(Transform target)
    {
        cinemachineCamera.Follow = target;
        SmoothTransitionOrthographicSize(10f, 1f);
    }

    public void SetFocusTarget(Transform target)
    {
        cinemachineCamera.Follow = target;
        SmoothTransitionOrthographicSize(5f, 1f);
    }

    private void SmoothTransitionOrthographicSize(float targetSize, float duration)
    {
        DOTween.To(() => cinemachineCamera.Lens.OrthographicSize, x => cinemachineCamera.Lens.OrthographicSize = x, targetSize, duration)
            .SetEase(Ease.OutSine);
    }
}
