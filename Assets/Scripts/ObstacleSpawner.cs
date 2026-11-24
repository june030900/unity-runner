using UnityEngine;

/// <summary>
/// 障碍生成器：按「已移动距离」生成，速度越快间距越大，保证可跳过
/// </summary>
public class ObstacleSpawner : MonoBehaviour
{
    float distanceSinceSpawn;
    float nextGap = GameConfig.FirstGap;

    void Update()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (gm.CurrentState != GameManager.State.Running || gm.ObstaclesRoot == null)
        {
            distanceSinceSpawn = 0f;
            nextGap = GameConfig.FirstGap;
            return;
        }

        distanceSinceSpawn += gm.Speed * Time.deltaTime;

        if (distanceSinceSpawn >= nextGap)
        {
            distanceSinceSpawn = 0f;
            nextGap = ComputeGap(gm.Speed);
            Obstacle.Create(gm.ObstaclesRoot, ViewBounds.Right + 1.5f);
        }
    }

    static float ComputeGap(float speed)
    {
        return GameConfig.MinGap
             + speed * GameConfig.GapPerSpeed
             + Random.Range(0f, GameConfig.GapJitter);
    }
}
