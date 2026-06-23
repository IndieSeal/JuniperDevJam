using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PathPoint
{
    public Transform transform;
    public float duration = 0;
}

public class Path : MonoBehaviour
{
    [SerializeField] private List<PathPoint> pathPoints = new List<PathPoint>();

    public PathPoint GetPathPoint(int index)
    {
        if(index >= pathPoints.Count) return null;
        return pathPoints[index];
    }

    void OnDrawGizmos()
    {
        DrawXXL.DrawBasics2D.LineString(pathPoints.Where(x => x != null).Select(x => (Vector2)x.transform.position).ToArray(), Color.green, width: 0.1f);
        foreach(var point in pathPoints) DrawXXL.DrawText.Write2D($"{point.transform.name}\n{point.duration}s", point.transform.position, size: 0.5f);
    }
}