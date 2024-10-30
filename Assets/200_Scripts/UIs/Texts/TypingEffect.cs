using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class TypingEffect : MonoBehaviour
{
    [SerializeField] private float defaultTypingSpeed = 0.05f;
    [SerializeField] private float wiggleAmount = 5f;
    [SerializeField] private float wiggleSpeed = 5f;

    private TextMeshProUGUI textComponent;

    private string fullText;
    private readonly List<int> wiggleIndices = new List<int>();
    private readonly Dictionary<float, WaitForSeconds> waitForSecondsCache = new Dictionary<float, WaitForSeconds>();

    private Regex speedTagRegex = new Regex(@"<speed=(?<value>[\d.]+)>");

    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        fullText = textComponent.text;
        textComponent.text = "";
        StartCoroutine(TypeTextCoroutine());
    }

    private void Update()
    {
        ApplyWiggleEffect();
    }

    private IEnumerator TypeTextCoroutine()
    {
        float currentTypingSpeed = defaultTypingSpeed;
        bool insideTag = false;
        bool insideWiggleTag = false;
        string tagContent = "";
        int charIndex = 0;

        for (int i = 0; i < fullText.Length; i++)
        {
            char letter = fullText[i];

            if (letter == '<')
            {
                insideTag = true;
                tagContent = "";
            }

            if (insideTag)
            {
                tagContent += letter;
                if (letter == '>')
                {
                    insideTag = false;
                    ProcessTag(tagContent, ref currentTypingSpeed, ref insideWiggleTag);
                }
            }
            else
            {
                textComponent.text += letter;
                if (insideWiggleTag)
                {
                    wiggleIndices.Add(charIndex);
                }
                charIndex++;
                ApplyWiggleEffect();
                yield return GetWaitForSeconds(currentTypingSpeed);
            }
        }
    }

    private WaitForSeconds GetWaitForSeconds(float seconds)
    {
        // 대기 시간을 캐시에서 가져오거나 새로 생성
        if (!waitForSecondsCache.TryGetValue(seconds, out WaitForSeconds waitForSeconds))
        {
            waitForSeconds = new WaitForSeconds(seconds);
            waitForSecondsCache[seconds] = waitForSeconds;
        }
        return waitForSeconds;
    }

    private void ProcessTag(string tagContent, ref float currentTypingSpeed, ref bool insideWiggleTag)
    {
        // 태그 내용 처리
        if (tagContent.StartsWith("<wiggle>"))
        {
            insideWiggleTag = true;
        }
        else if (tagContent.StartsWith("</wiggle>"))
        {
            insideWiggleTag = false;
        }
        else if (tagContent.StartsWith("<speed="))
        {
            Match match = speedTagRegex.Match(tagContent);
            if (match.Success)
            {
                string speedValue = match.Groups["value"].Value;
                if (float.TryParse(speedValue, out float newSpeed))
                {
                    currentTypingSpeed = newSpeed;
                }
            }
        }
        else if (tagContent.StartsWith("</speed>"))
        {
            currentTypingSpeed = defaultTypingSpeed;
        }
        else
        {
            textComponent.text += tagContent;
        }
    }

    private void ApplyWiggleEffect()
    {
        textComponent.ForceMeshUpdate();
        TMP_TextInfo textInfo = textComponent.textInfo;

        // wiggle 효과를 적용할 문자들에 대해 처리
        for (int i = 0; i < wiggleIndices.Count; i++)
        {
            int charIndex = wiggleIndices[i];
            TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];

            if (!charInfo.isVisible)
                continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            // wiggle 효과를 위한 offset 계산
            Vector3 offset = new Vector3(
                Mathf.Sin(Time.time * wiggleSpeed + charIndex) * wiggleAmount,
                Mathf.Cos(Time.time * wiggleSpeed + charIndex) * wiggleAmount,
                0
            );

            // 각 정점에 offset 적용
            for (int j = 0; j < 4; j++)
            {
                vertices[vertexIndex + j] += offset;
            }
        }

        // 변경된 정점 정보를 업데이트
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textComponent.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}
