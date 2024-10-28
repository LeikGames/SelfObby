using UnityEngine;

public static class VectorExtensions
{
    /// <summary>
    /// Rotate a vector by the given degrees.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="degrees"></param>
    /// <returns></returns>
    public static Vector2 RotateDeg(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    /// <summary>
    /// Rotate the vector by the given angle in radians.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="radians"></param>
    /// <returns></returns>
    public static Vector2 Rotate(this Vector2 v, float radians)
    {
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    /// <summary>
    /// Calculate the angle of the given vector.
    /// </summary>
    /// <param name="v"></param>
    /// <returns>The angle in degrees</returns>
    public static float ToAngleDeg(this Vector2 v)
    {
        return ToAngle(v) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// Calculate the angle of the given vector.
    /// </summary>
    /// <param name="v"></param>
    /// <returns>The angle in radians</returns>
    public static float ToAngle(this Vector2 v)
    {
        return Mathf.Atan2(v.y, v.x);
    }

    public static readonly Vector2[] Vec2Dirs = new Vector2[]
    {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right,
        Vector2.zero
    };

    public static Vector2 FromDirection(EDirection direction)
    {
        return Vec2Dirs[(int)direction];
    }

    public static Vector2 FromSide(ESide direction)
    {
        return Vec2Dirs[(int)direction];
    }

    public static Vector2 Clamp(this Vector2 v, Vector2 min, Vector2 max)
    {
        return new Vector2(Mathf.Clamp(v.x, min.x, max.x), Mathf.Clamp(v.y, min.y, max.y));
    }

    public static Vector3 Clamp(this Vector3 v, Vector3 min, Vector3 max)
    {
        return new Vector3(
            Mathf.Clamp(v.x, min.x, max.x),
            Mathf.Clamp(v.y, min.y, max.y),
            Mathf.Clamp(v.z, min.z, max.z)
        );
    }
}
