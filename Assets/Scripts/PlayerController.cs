using UnityEngine;

/// <summary>
/// 玩家角色：二段逻辑的单跳、落地检测、挤压拉伸动画
/// </summary>
public class PlayerController : MonoBehaviour
{
    public Transform Visual;

    Rigidbody2D rb;
    bool grounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (InputHelper.Pressed())
        {
            if (gm.CurrentState == GameManager.State.Ready)
            {
                gm.StartRun();
            }
            else if (gm.CurrentState == GameManager.State.GameOver)
            {
                gm.Restart();
            }
            else
            {
                TryJump();
            }
        }

        UpdateSquash();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<GroundMarker>() != null) grounded = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<GroundMarker>() != null) grounded = false;
    }

    public void TryJump()
    {
        if (!grounded) return;

        SetVelocityY(GameConfig.JumpVelocity);
        grounded = false;
        SoundFX.PlayJump();
    }

    /// <summary>重置到指定位置（重开时用）</summary>
    public void ResetTo(Vector3 position)
    {
        transform.position = position;
        if (Visual != null) Visual.localScale = Vector3.one;

        SetVelocity(Vector2.zero);
        grounded = false;
    }

    /// <summary>根据竖直速度做轻微挤压/拉伸，提升手感</summary>
    void UpdateSquash()
    {
        if (Visual == null) return;

        float vy = GetVelocityY();
        float scaleY = Mathf.Clamp(1f + vy * 0.012f, 0.9f, 1.1f);
        Visual.localScale = new Vector3(1f / scaleY, scaleY, 1f);
    }

    float GetVelocityY()
    {
#if UNITY_6000_0_OR_NEWER
        return rb.linearVelocity.y;
#else
        return rb.velocity.y;
#endif
    }

    void SetVelocityY(float y)
    {
#if UNITY_6000_0_OR_NEWER
        var v = rb.linearVelocity;
        v.y = y;
        rb.linearVelocity = v;
#else
        var v = rb.velocity;
        v.y = y;
        rb.velocity = v;
#endif
    }

    void SetVelocity(Vector2 velocity)
    {
#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = velocity;
#else
        rb.velocity = velocity;
#endif
    }
}
