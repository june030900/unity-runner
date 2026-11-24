using UnityEngine;

/// <summary>
/// 通用滚动对象：随游戏速度向左移动，超出屏幕左侧后按 wrapSpan 循环，
/// 地面接缝与远景建筑共用这段逻辑
/// </summary>
public class ScrollingObject : MonoBehaviour
{
    public float speedFactor = 1f;
    public float wrapSpan = 20f;
    public float wrapLeft = -10f;

    void Update()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (gm.CurrentState == GameManager.State.Running)
        {
            transform.position += Vector3.left * (gm.Speed * speedFactor * Time.deltaTime);
        }

        if (transform.position.x < wrapLeft)
        {
            transform.position += Vector3.right * wrapSpan;
        }
    }
}
