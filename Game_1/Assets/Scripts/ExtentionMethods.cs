using UnityEngine;

public static class ExtentionMethods
{
    public static bool HasComponent<T>(this GameObject gameObject)
    {
        return gameObject.TryGetComponent<T>(out T component);
    }

    public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);
    }

    public static float Normalize(this float value)
    {
        if (value == 0f)
            return 0f;
        else
            return value / Mathf.Abs(value);
    }

    public static float Positive(this float value)
    {
        if (value < 0f)
            return value * -1f;
        else
            return value;
    }

    public static int Normalize(this int value)
    {
        if (value == 0f)
            return 0;
        else
            return value / Mathf.Abs(value);
    }

    public static int Positive(this int value)
    {
        if (value < 0f)
            return value * -1;
        else
            return value;
    }

    public static Vector2 GetRandomLeftRight() => ((Random.Range(0, 10) % 2) == 0) ? Vector2.left : Vector2.right;

}
