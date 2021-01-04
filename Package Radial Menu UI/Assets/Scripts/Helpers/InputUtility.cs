using UnityEngine;

public class InputUtility
{
    public static bool IsTouched() {
        return Input.GetMouseButtonDown(0);
    }

    public static bool IsTouching() {
        return Input.GetMouseButton(0);
    }

    public static bool IsReleased() {
        return Input.GetMouseButtonUp(0);
    }
}

