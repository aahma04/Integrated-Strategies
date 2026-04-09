using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RangeCirclePreview : MonoBehaviour
{
    private SpriteRenderer sr;
    private static Sprite circleSprite;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        if (circleSprite == null)
        {
            circleSprite = CreateCircleSprite(256);
        }

        sr.sprite = circleSprite;
        sr.color = new Color(0f, 0.7f, 1f, 0.18f);
        sr.sortingOrder = 200;
    }

    public void SetRadius(float radius)
    {
        float diameter = radius * 2f;
        transform.localScale = new Vector3(diameter, diameter, 1f);
    }

    public void SetColor(Color color)
    {
        sr.color = color;
    }

    public void Show(bool show)
    {
        sr.enabled = show;
    }

    private Sprite CreateCircleSprite(int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;

        Vector2 center = new Vector2((size - 1) / 2f, (size - 1) / 2f);
        float radius = size / 2f - 1f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);

                if (dist <= radius)
                {
                    tex.SetPixel(x, y, Color.white);
                }
                else
                {
                    tex.SetPixel(x, y, Color.clear);
                }
            }
        }

        tex.Apply();

        return Sprite.Create(
            tex,
            new Rect(0, 0, size, size),
            new Vector2(0.5f, 0.5f),
            size
        );
    }
}