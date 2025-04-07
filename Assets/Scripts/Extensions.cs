using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class that provides some helpful methods.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Randomly shuffles the array elements together.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    public static void Shuffle<T>(this T[] data)
    {
        for (int i = data.Length - 1; i >= 1; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = data[j];
            data[j] = data[i];
            data[i] = temp;
        }
    }

    /// <summary>
    /// Pick a random element from the array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns>A random element.</returns>
    public static T PickRandomElement<T>(this T[] data)
    {
        return data.Length == 1 ? data[0] : data[Random.Range(0, data.Length - 1)];
    }

    /// <summary>
    /// Randomly shuffles the list elements together.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    public static void Shuffle<T>(this List<T> data)
    {
        for (int i = data.Count - 1; i >= 1; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = data[j];
            data[j] = data[i];
            data[i] = temp;
        }
    }

    /// <summary>
    /// Pick a random element from list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns>A random element.</returns>
    public static T PickRandomElement<T>(this List<T> data)
    {
        return data.Count == 1 ? data[0] : data[Random.Range(0, data.Count - 1)];
    }

    /// <summary>
    /// Pick random element from IEnumerable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="col"></param>
    /// <returns></returns>
    public static T PickOne<T>(this IEnumerable<T> col)
    {
        T[] enumerable = col as T[] ?? col.ToArray();
        return enumerable[Random.Range(0, enumerable.Count())];
    }

    public static bool IsEmpty<T>(this IEnumerable<T> col) => !col.Any();


    public static Vector3 SetX(this Vector3 value, float x)
    {
        value.x = x;
        return value;
    }

    public static Vector3 SetY(this Vector3 value, float y)
    {
        value.y = y;
        return value;
    }

    public static Vector3 SetZ(this Vector3 value, float z)
    {
        value.z = z;
        return value;
    }

    public static Vector3 AddX(this Vector3 value, float x)
    {
        value.x += x;
        return value;
    }

    public static Vector3 AddY(this Vector3 value, float y)
    {
        value.y += y;
        return value;
    }

    public static Vector3 AddZ(this Vector3 value, float z)
    {
        value.z += z;
        return value;
    }

    public static Vector2 ToVector2(this Vector3 vector3)
    {
        return new Vector2
        {
            x = vector3.x,
            y = vector3.z
        };
    }

    public static Vector3 ToVector3(this Vector2 vector2, float y = 0)
    {
        return new Vector3
        {
            x = vector2.x,
            y = y,
            z = vector2.y
        };
    }
}
