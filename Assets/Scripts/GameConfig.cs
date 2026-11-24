using UnityEngine;

/// <summary>
/// 游戏可调参数集中管理，方便平衡与调参
/// </summary>
public static class GameConfig
{
    // 地面表面所在的世界坐标 Y
    public const float GroundTop = -3f;

    // 玩家
    public const float PlayerWidth = 0.8f;
    public const float PlayerHeight = 1f;
    public const float PlayerStartX = -4.5f;
    public const float GravityScale = 3.6f;
    public const float JumpVelocity = 11.5f;

    // 滚动速度
    public const float StartSpeed = 6f;
    public const float MaxSpeed = 14f;
    public const float Acceleration = 0.12f;

    // 计分：每移动 1 个世界单位得多少分
    public const float ScorePerUnit = 10f;

    // 障碍生成间距（按已经移动的距离计算）
    public const float FirstGap = 6f;
    public const float MinGap = 4.6f;
    public const float GapPerSpeed = 0.45f;
    public const float GapJitter = 2f;

    // 相机
    public const float CameraSize = 5f;

    // 本地最高分存储 key
    public const string HighScoreKey = "UnityRunner_HighScore";
}
