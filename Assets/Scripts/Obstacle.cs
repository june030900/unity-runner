using UnityEngine;

/// <summary>
/// 障碍物：向左移动、出屏销毁、触发碰撞即游戏结束
/// </summary>
public class Obstacle : MonoBehaviour
{
    static readonly Vector2[] SizeTable =
    {
        new Vector2(0.7f, 0.9f),
        new Vector2(0.85f, 1.15f),
        new Vector2(0.8f, 1.45f),
        new Vector2(1.4f, 0.7f),
    };

    /// <summary>在指定 x 处创建一个随机尺寸的障碍物</summary>
    public static Obstacle Create(Transform parent, float x)
    {
        Vector2 size = SizeTable[Random.Range(0, SizeTable.Length)];

        var go = new GameObject("Obstacle");
        go.transform.SetParent(parent, false);
        go.transform.position = new Vector3(x, GameConfig.GroundTop + size.y * 0.5f, 0f);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteFactory.ObstacleSprite(size.x, size.y);
        sr.sortingOrder = 8;

        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(size.x * 0.82f, size.y * 0.94f);

        return go.AddComponent<Obstacle>();
    }

    void Update()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (gm.CurrentState == GameManager.State.Running)
        {
            transform.position += Vector3.left * (gm.Speed * Time.deltaTime);
        }

        if (transform.position.x < ViewBounds.Left - 4f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.Instance == null) return;
        if (other.GetComponent<PlayerController>() != null)
        {
            GameManager.Instance.GameOver();
        }
    }
}
