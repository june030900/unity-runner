using UnityEngine;

/// <summary>
/// 界面：用 IMGUI 绘制分数、开始/结束界面，无需 Canvas/Text 组件
/// </summary>
public class GameUI : MonoBehaviour
{
    GUIStyle scoreStyle;
    GUIStyle rightStyle;
    GUIStyle titleStyle;
    GUIStyle bodyStyle;
    GUIStyle smallStyle;

    float scale = 1f;

    void EnsureStyles()
    {
        scale = Mathf.Max(1f, Screen.height / 720f);

        if (scoreStyle == null)
        {
            var baseStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };

            scoreStyle = new GUIStyle(baseStyle) { alignment = TextAnchor.MiddleLeft };
            rightStyle = new GUIStyle(baseStyle) { alignment = TextAnchor.MiddleRight };
            titleStyle = new GUIStyle(baseStyle) { alignment = TextAnchor.MiddleCenter };
            bodyStyle = new GUIStyle(baseStyle) { alignment = TextAnchor.MiddleCenter };
            smallStyle = new GUIStyle(baseStyle)
            {
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleCenter,
            };

            scoreStyle.normal.textColor = Color.white;
            rightStyle.normal.textColor = Color.white;
            titleStyle.normal.textColor = Color.white;
            bodyStyle.normal.textColor = Color.white;
            smallStyle.normal.textColor = new Color(0.72f, 0.83f, 0.95f);
        }

        scoreStyle.fontSize = Mathf.RoundToInt(22 * scale);
        rightStyle.fontSize = Mathf.RoundToInt(22 * scale);
        titleStyle.fontSize = Mathf.RoundToInt(46 * scale);
        bodyStyle.fontSize = Mathf.RoundToInt(28 * scale);
        smallStyle.fontSize = Mathf.RoundToInt(18 * scale);
    }

    void OnGUI()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        EnsureStyles();

        float w = Screen.width;
        float h = Screen.height;

        // 顶部 HUD
        GUI.Label(new Rect(24 * scale, 16 * scale, 240 * scale, 36 * scale), "得分 " + gm.Score, scoreStyle);

        int best = Mathf.Max(gm.HighScore, gm.Score);
        GUI.Label(new Rect(w - 264 * scale, 16 * scale, 240 * scale, 36 * scale), "最高分 " + best, rightStyle);

        if (gm.CurrentState == GameManager.State.Ready)
        {
            GUI.Label(new Rect(0, h * 0.26f, w, 70 * scale), "像素跑酷", titleStyle);
            GUI.Label(new Rect(0, h * 0.38f, w, 44 * scale), "点击屏幕 / 空格 开始", bodyStyle);
            GUI.Label(new Rect(0, h * 0.45f, w, 34 * scale), "起跳躲避障碍，跑得越远分越高", smallStyle);
        }
        else if (gm.CurrentState == GameManager.State.GameOver)
        {
            GUI.color = new Color(0f, 0f, 0f, 0.6f);
            GUI.DrawTexture(new Rect(0f, 0f, w, h), Texture2D.whiteTexture);
            GUI.color = Color.white;

            GUI.Label(new Rect(0, h * 0.22f, w, 70 * scale), "游戏结束", titleStyle);

            if (gm.IsNewRecord)
            {
                var recordStyle = new GUIStyle(bodyStyle);
                recordStyle.normal.textColor = new Color(1f, 0.9f, 0.3f);
                GUI.Label(new Rect(0, h * 0.32f, w, 40 * scale), "新纪录！", recordStyle);
            }

            GUI.Label(new Rect(0, h * 0.40f, w, 44 * scale), "本局得分 " + gm.Score, bodyStyle);
            GUI.Label(new Rect(0, h * 0.47f, w, 34 * scale), "最高分 " + gm.HighScore, smallStyle);
            GUI.Label(new Rect(0, h * 0.58f, w, 34 * scale), "点击屏幕 / 空格 重新开始", smallStyle);
        }
    }
}
