using UnityEngine;

/// <summary>
/// 输入兼容层：
/// 同时支持旧版 Input Manager 与新版 Input System Package，
/// 避免不同 Unity 模板下 "Input.GetKeyDown 无法使用" 的报错。
/// </summary>
public static class InputHelper
{
    /// <summary>本帧是否按下了「跳跃 / 开始 / 重开」</summary>
    public static bool Pressed()
    {
#if ENABLE_INPUT_SYSTEM
        var keyboard = UnityEngine.InputSystem.Keyboard.current;
        if (keyboard != null &&
            (keyboard.spaceKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame))
        {
            return true;
        }

        var mouse = UnityEngine.InputSystem.Mouse.current;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame) return true;

        var touch = UnityEngine.InputSystem.Touchscreen.current;
        if (touch != null && touch.primaryTouch.press.wasPressedThisFrame) return true;

        return false;
#elif ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKeyDown(KeyCode.Space)
            || Input.GetKeyDown(KeyCode.UpArrow)
            || Input.GetMouseButtonDown(0);
#else
        return false;
#endif
    }
}
