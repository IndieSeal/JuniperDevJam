using UnityEngine;

public class CursorManager : Singleton<CursorManager>
{
    [SerializeField] private Texture2D normalCursor;
    [SerializeField] private Texture2D pointCursor;

    protected override void Awake()
    {
        base.Awake();
        
        SetNormalCursor();
    }

    public void SetNormalCursor()
    {
        Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
    }
    public void SetPointCursor()
    {
        Cursor.SetCursor(pointCursor, Vector2.zero, CursorMode.Auto);
    }
}