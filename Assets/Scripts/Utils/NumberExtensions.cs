using UnityEngine;

public static class NumberExtensions
{
    /// <summary>
    /// Maps one range to another.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="from1"></param>
    /// <param name="to1"></param>
    /// <param name="from2"></param>
    /// <param name="to2"></param>
    /// <returns></returns>
    public static float Map(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    /// <summary>
    /// Calculate a vector from a given radian angle.
    /// </summary>
    /// <param name="radians"></param>
    /// <param name="length">Length of the resulting vector</param>
    /// <returns></returns>
    ///
    public static Vector2 ToVector2(this float radians, float length = 1)
    {
        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * length;
    }

    public static float Increment(this float value, float increment)
    {
        return Mathf.Round(value / increment) * increment;
    }

    public static float Pow2(this float value)
    {
        return value * value;
    }
}
