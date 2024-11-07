using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DarkEvent : MonoBehaviour
{
    [SerializeField] private bool initialDark;
    [SerializeField] private float animationDuration = 2f;

    public Player player;

    public bool isDark;
    private readonly Dictionary<Light2D, float> lightDictionary = new Dictionary<Light2D, float>();

    private void Start()
    {
        isDark = initialDark;

        // 플레이어를 제외한 씬에 있는 Light2D 컴포넌트를 모두 찾아서 딕셔너리에 저장
        Light2D[] allLights = FindObjectsOfType<Light2D>();
        foreach (Light2D light in allLights)
        {
            if (!light.transform.IsChildOf(player.transform))
            {
                lightDictionary[light] = light.intensity;
            }
        }

        if (initialDark)
        {
            UpdateLightIntensities(true, true);
        }
    }

    public void UpdateLightIntensities(bool isDark, bool immediate = false)
    {
        this.isDark = isDark;

        foreach (KeyValuePair<Light2D, float> entry in lightDictionary)
        {
            Light2D light = entry.Key;
            float targetIntensity = isDark ? 0f : entry.Value;

            if (immediate)
            {
                light.intensity = targetIntensity;
            }
            else
            {
                DOTween.To(() => light.intensity, x => light.intensity = x, targetIntensity, animationDuration);
            }
        }
    }
}