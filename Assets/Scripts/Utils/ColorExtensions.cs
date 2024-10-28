using UnityEngine;

public static class ColorExtensions
{
    public static Color MoveTowards(this Color value, Color target, float maxDelta)
    {
        value.r = Mathf.MoveTowards(value.r, target.r, maxDelta);
        value.g = Mathf.MoveTowards(value.g, target.g, maxDelta);
        value.b = Mathf.MoveTowards(value.b, target.b, maxDelta);
        value.a = Mathf.MoveTowards(value.a, target.a, maxDelta);
        return value;
    }

    public static float Snap(this float value, float increment)
    {
        return Mathf.Round(value / increment) * increment;
    }
}
