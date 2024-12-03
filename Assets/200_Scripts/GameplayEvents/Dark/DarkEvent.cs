using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DarkEvent : MonoBehaviour
{
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float intervalDuration = 1f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float defaultLightRadiusMultiplier = 0.5f;
    [SerializeField] private float maxLightRadiusMultiplier = 1.2f;

    private Player player;

    private readonly Dictionary<Light2D, float> playerLightDictionary = new Dictionary<Light2D, float>();
    private readonly Dictionary<Light2D, Sequence> sequenceDictionary = new Dictionary<Light2D, Sequence>();

    public void InitialPlayer(Player player)
    {
        this.player = player;

        playerLightDictionary.Clear();
        sequenceDictionary.Clear();

        Light2D[] allLights = FindObjectsOfType<Light2D>();
        foreach (Light2D light in allLights)
        {
            if (light.transform.IsChildOf(player.transform))
            {
                playerLightDictionary[light] = light.pointLightOuterRadius;
                light.pointLightOuterRadius *= defaultLightRadiusMultiplier;
            }
            else if (!light.CompareTag("EnterExit"))
            {
                light.intensity = 0f;
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
            sequence.Append(DOTween.To(() => light.pointLightOuterRadius, x => light.pointLightOuterRadius = x, originalPointLightOuterRadius * maxLightRadiusMultiplier, fadeInDuration));
            sequence.AppendInterval(intervalDuration);
            sequence.Append(DOTween.To(() => light.pointLightOuterRadius, x => light.pointLightOuterRadius = x, originalPointLightOuterRadius * defaultLightRadiusMultiplier, fadeOutDuration));
            sequenceDictionary[light] = sequence;
        }
    }
}
