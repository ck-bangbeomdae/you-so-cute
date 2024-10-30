using System.Collections;

public class NoneTransition : BaseTransition
{
    public override IEnumerator FadeIn()
    {
        yield return null;
    }

    public override IEnumerator FadeOut()
    {
        yield return null;
    }
}
