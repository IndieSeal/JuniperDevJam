using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public static class Utilities
{
    public static Vector3 GetScreenToWorldPoint(Vector3 position) => Camera.main.ScreenToWorldPoint(position);
    public static Vector2 Get2DScreenToWorldPoint(Vector2 position) => GetScreenToWorldPoint(position);

    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
    public static Vector2 Get2DMouseWorldPosition() => GetMouseWorldPosition();

    public static T GetRandomOf<T>(this List<T> myList)
    {
        if (myList.Count == 0) return default;

        return myList[myList.GetRandomIndexOf()];
    }
    public static int GetRandomIndexOf<T>(this List<T> myList) => Random.Range(0, myList.Count);

    public static T GetRandomOf<T>(this T[] array)
    {
        if (array.Length == 0) return default;

        return array[array.GetRandomIndexOf()];
    }
    private static int GetRandomIndexOf<T>(this T[] array) => Random.Range(0, array.Length);

    public static T GetRandomOf<T>(this T[,] array)
    {
        if (array.Length == 0) return default;

        Vector2Int rand = GetRandomIndexOf(array);
        return array[rand.x, rand.y];
    }
    private static Vector2Int GetRandomIndexOf<T>(this T[,] array) => new Vector2Int(Random.Range(0, array.GetLength(0)), Random.Range(0, array.GetLength(1)));

    public static Vector2 GetRandom(Vector2 start, Vector2 end) => new Vector2(Random.Range(start.x, end.x), Random.Range(start.y, end.y));

    public static List<T> ConvertToList<T>(this T component) => new List<T>() { component };

    public static Vector3 ConvertToVector(this float myFloat) => new Vector3(myFloat, myFloat, myFloat);

    public static Quaternion LookAt2D(this Transform user, Vector3 target, float offset = 0, float minAngle = 0, float maxAngle = 360) => LookAt2D(user.position, target, minAngle, maxAngle, offset);
    public static Quaternion LookAt2D(Vector2 user, Vector2 target, float minAngle, float maxAngle, float offset = 0)
    {
        float angle = LookAt2DAngle(user, target, offset);
        
        float clampedAngle = Mathf.Clamp(angle, minAngle, maxAngle);
        Quaternion rotation = Quaternion.Euler(0, 0, clampedAngle);
        return rotation;
    }

    public static Quaternion LookAt2D(Vector2 user, Vector2 target, float offset = 0)
    {
        float angle = LookAt2DAngle(user, target, offset);
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        return rotation;
    }

    public static float LookAt2DAngle(Vector2 user, Vector2 target, float offset = 0)
    {
        Vector2 direction2D = target - user;
        return offset + (Mathf.Atan2(direction2D.y, direction2D.x) * Mathf.Rad2Deg);
    }
}