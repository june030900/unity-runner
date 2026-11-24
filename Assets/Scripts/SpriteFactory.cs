using UnityEngine;

/// <summary>
/// 程序化生成精灵（纯代码画图），整个项目不依赖任何美术资源
/// </summary>
public static class SpriteFactory
{
    public const float PixelsPerUnit = 32f;

    /// <summary>1x1 单位的纯色方块，可自由缩放</summary>
    public static Sprite Solid(Color color)
    {
        var tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
        var pixels = new Color32[16];
        Color32 c = color;
        for (int i = 0; i < pixels.Length; i++) pixels[i] = c;

        tex.SetPixels32(pixels);
        tex.Apply();
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;

        return Sprite.Create(tex, new Rect(0f, 0f, 4f, 4f), new Vector2(0.5f, 0.5f), 4f);
    }

    /// <summary>圆角矩形精灵（带描边）</summary>
    public static Sprite RoundedRect(float width, float height, Color fill, Color outline, float radius = 0.12f, int outlineWidth = 2)
    {
        int pw = Mathf.Max(6, Mathf.RoundToInt(width * PixelsPerUnit));
        int ph = Mathf.Max(6, Mathf.RoundToInt(height * PixelsPerUnit));

        var tex = MakeRoundedTexture(pw, ph, radius * PixelsPerUnit, fill, outline, outlineWidth);
        return Sprite.Create(tex, new Rect(0f, 0f, pw, ph), new Vector2(0.5f, 0.5f), PixelsPerUnit);
    }

    /// <summary>玩家角色：圆角身体 + 两只眼睛</summary>
    public static Sprite PlayerSprite(float width, float height)
    {
        int pw = Mathf.RoundToInt(width * PixelsPerUnit);
        int ph = Mathf.RoundToInt(height * PixelsPerUnit);

        var body = new Color(0.32f, 0.78f, 0.98f);
        var edge = new Color(0.05f, 0.33f, 0.55f);
        var tex = MakeRoundedTexture(pw, ph, 0.2f * PixelsPerUnit, body, edge, 2);

        float eyeRadius = pw * 0.1f;
        int eyeY = Mathf.RoundToInt(ph * 0.66f);
        FillCircle(tex, Mathf.RoundToInt(pw * 0.34f), eyeY, eyeRadius, Color.white);
        FillCircle(tex, Mathf.RoundToInt(pw * 0.66f), eyeY, eyeRadius, Color.white);
        FillCircle(tex, Mathf.RoundToInt(pw * 0.35f), eyeY, eyeRadius * 0.5f, new Color(0.06f, 0.09f, 0.15f));
        FillCircle(tex, Mathf.RoundToInt(pw * 0.67f), eyeY, eyeRadius * 0.5f, new Color(0.06f, 0.09f, 0.15f));
        tex.Apply();

        return Sprite.Create(tex, new Rect(0f, 0f, pw, ph), new Vector2(0.5f, 0.5f), PixelsPerUnit);
    }

    /// <summary>障碍物：红色圆角方块</summary>
    public static Sprite ObstacleSprite(float width, float height)
    {
        var fill = new Color(0.95f, 0.38f, 0.32f);
        var edge = new Color(0.42f, 0.1f, 0.12f);
        return RoundedRect(width, height, fill, edge, 0.1f, 3);
    }

    /// <summary>用 SDF 方式生成带抗锯齿的圆角矩形纹理</summary>
    static Texture2D MakeRoundedTexture(int pw, int ph, float radius, Color fill, Color outline, int outlineWidth)
    {
        var tex = new Texture2D(pw, ph, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;

        var pixels = new Color32[pw * ph];
        float halfW = pw * 0.5f;
        float halfH = ph * 0.5f;
        radius = Mathf.Clamp(radius, 0f, Mathf.Min(halfW, halfH));

        for (int y = 0; y < ph; y++)
        {
            for (int x = 0; x < pw; x++)
            {
                float px = x + 0.5f - halfW;
                float py = y + 0.5f - halfH;

                // 圆角矩形的有符号距离（像素单位，负数在内部）
                float qx = Mathf.Abs(px) - (halfW - radius);
                float qy = Mathf.Abs(py) - (halfH - radius);
                float outside = new Vector2(Mathf.Max(qx, 0f), Mathf.Max(qy, 0f)).magnitude;
                float inside = Mathf.Min(Mathf.Max(qx, qy), 0f);
                float dist = outside + inside - radius;

                Color color = dist > -outlineWidth ? outline : fill;
                color.a *= Mathf.Clamp01(0.5f - dist);
                pixels[y * pw + x] = color;
            }
        }

        tex.SetPixels32(pixels);
        tex.Apply();
        return tex;
    }

    static void FillCircle(Texture2D tex, int cx, int cy, float radius, Color color)
    {
        int minX = Mathf.Max(0, Mathf.FloorToInt(cx - radius));
        int maxX = Mathf.Min(tex.width - 1, Mathf.CeilToInt(cx + radius));
        int minY = Mathf.Max(0, Mathf.FloorToInt(cy - radius));
        int maxY = Mathf.Min(tex.height - 1, Mathf.CeilToInt(cy + radius));

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                float dx = x + 0.5f - cx;
                float dy = y + 0.5f - cy;
                if (dx * dx + dy * dy <= radius * radius) tex.SetPixel(x, y, color);
            }
        }
    }
}
