using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseTransition : MonoBehaviour
{
    protected Image fadeImage;

    private void Awake()
    {
        fadeImage = GetComponentInChildren<Image>();
    }

    public abstract IEnumerator FadeIn();

    public abstract IEnumerator FadeOut();
}
