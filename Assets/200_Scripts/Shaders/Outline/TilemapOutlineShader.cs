using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class TilemapOutlineShader : MonoBehaviour
{
    public Color color = Color.white;

    [Range(0, 16)]
    public int outlineSize = 1;

    private TilemapRenderer spriteRenderer;

    void OnEnable()
    {
        spriteRenderer = GetComponent<TilemapRenderer>();

        UpdateOutline(true);
    }

    void OnDisable()
    {
        UpdateOutline(false);
    }

    void Update()
    {
        UpdateOutline(true);
    }

    void UpdateOutline(bool outline)
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();

        spriteRenderer.GetPropertyBlock(mpb);

        mpb.SetFloat("_Outline", outline ? 1f : 0f);
        mpb.SetColor("_OutlineColor", color);
        mpb.SetFloat("_OutlineSize", outlineSize);

        spriteRenderer.SetPropertyBlock(mpb);
    }
}
