using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DarkEvent : MonoBehaviour
{
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float intervalDuration = 1f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float maxLightRadiusMultiplier = 1.2f;
    [SerializeField] private float minLightRadiusMultiplier = 0.5f;

    [HideInInspector] public Player player;

    private readonly Dictionary<Light2D, float> playerLightDictionary = new Dictionary<Light2D, float>();
    private readonly Dictionary<Light2D, Sequence> sequenceDictionary = new Dictionary<Light2D, Sequence>();

    private void Start()
    {
        // 플레이어를 제외한 씬에 있는 Light2D 컴포넌트를 모두 찾아서 딕셔너리에 저장
        Light2D[] allLights = FindObjectsOfType<Light2D>();
        foreach (Light2D light in allLights)
        {
            if (!light.transform.IsChildOf(player.transform))
            {
                light.intensity = 0f;
            }
            else
            {
                playerLightDictionary[light] = light.pointLightOuterRadius * maxLightRadiusMultiplier;
                light.pointLightOuterRadius *= minLightRadiusMultiplier;
            }
        }
    }

    public void TriggerBright()
    {
        foreach (KeyValuePair<Light2D, float> entry in playerLightDictionary)
        {
            Light2D light = entry.Key;
            float originalPointLightOuterRadius = entry.Value;

            // 기존 Sequence가 있으면 중지하고 제거
            if (sequenceDictionary.TryGetValue(light, out Sequence existingSequence))
            {
                existingSequence.Kill();
                sequenceDictionary.Remove(light);
            }

            // 새로운 Sequence 생성 및 저장
            Sequence sequence = DOTween.Sequence();
            sequence.Append(DOTween.To(() => light.pointLightOuterRadius, x => light.pointLightOuterRadius = x, originalPointLightOuterRadius, fadeInDuration));
            sequence.AppendInterval(intervalDuration);
            sequence.Append(DOTween.To(() => light.pointLightOuterRadius, x => light.pointLightOuterRadius = x, originalPointLightOuterRadius * minLightRadiusMultiplier, fadeOutDuration));
            sequenceDictionary[light] = sequence;
        }
    }
}
