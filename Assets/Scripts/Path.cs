using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PathPoint
{
    public Transform transform;
    public float duration = 0;
    public bool checkpoint;
    [ShowIf("checkpoint")] public float zoom = 15;
    [Space]
    [Tooltip("Won't call even after a reset")] public bool callOnce;
    public UnityEvent unityEvents = new UnityEvent();
    private bool called = false;

    public void CallEvents()
    {
        if(called) return;
        
        unityEvents?.Invoke();
        called = true;
    }

    public void LevelReset()
    {
        if(callOnce) return;
        called = false;
    }
}

public class Path : MonoBehaviour
{
    [SerializeField] private List<PathPoint> pathPoints = new List<PathPoint>();

    void Awake()
    {
        GameManager.OnResetLevel += LevelReset;
    }

    private void LevelReset()
    {
        foreach(PathPoint point in pathPoints) point.LevelReset();
    }

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