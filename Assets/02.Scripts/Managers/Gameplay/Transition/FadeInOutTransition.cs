using System.Collections;
using UnityEngine;

public class FadeInOutTransition : BaseTransition
{
    [Tooltip("페이드 인/아웃 색상")]
    public Color fadeColor = new Color(0f, 0f, 0f);

    [Range(1, 100), Tooltip("페이드 인/아웃 속도: 높을수록 빠름")]
    public byte stepRate = 10;

    private float step;

    private void Start()
    {
        step = stepRate * 0.001f;
    }

    public override IEnumerator FadeIn()
    {
        // FadeImage의 색상을 가져와서 알파 값을 0으로 설정
        Color color = fadeImage.color;
        color.a = 0f;
        fadeImage.color = color;

        // 알파 값을 점진적으로 증가시키며 페이드 인
        for (float alpha = fadeImage.color.a; alpha < 1f; alpha += step)
        {
            fadeColor.a = alpha;
            fadeImage.color = fadeColor;
            yield return null;
        }

        // 최종적으로 알파 값을 1로 설정
        fadeColor.a = 1f;
        fadeImage.color = fadeColor;
    }

    public override IEnumerator FadeOut()
    {
        // FadeImage의 색상을 가져와서 알파 값을 1로 설정
        Color color = fadeImage.color;
        color.a = 1f;
        fadeImage.color = color;

        // 알파 값을 점진적으로 감소시키며 페이드 아웃
        for (float alpha = fadeImage.color.a; alpha > 0; alpha -= step)
        {
            fadeColor.a = alpha;
            fadeImage.color = fadeColor;
            yield return null;
        }

        // 최종적으로 알파 값을 0으로 설정
        fadeColor.a = 0;
        fadeImage.color = fadeColor;
    }
}
