using UnityEngine;

/// <summary>
/// 相机可视范围（世界坐标），供生成物/回收物统一使用，避免写死坐标
/// </summary>
public static class ViewBounds
{
    public static float Left;
    public static float Right;
    public static float Bottom;
    public static float Top;

    static Camera cachedCamera;

    public static void Refresh()
    {
        if (cachedCamera == null) cachedCamera = Camera.main;
        if (cachedCamera == null) return;

        Left = cachedCamera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x;
        Right = cachedCamera.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x;
        Bottom = cachedCamera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).y;
        Top = cachedCamera.ViewportToWorldPoint(new Vector3(0f, 1f, 0f)).y;
    }

    public static float Width => Right - Left;
    public static float Height => Top - Bottom;
}
