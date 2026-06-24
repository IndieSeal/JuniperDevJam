using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorManager : Singleton<CursorManager>
{
    [SerializeField] private Image cursor;
    
    [SerializeField] private Sprite normalCursor;
    [SerializeField] private Sprite pointCursor;

    protected override void Awake()
    {
        base.Awake();
        
        SetNormalCursor();
    }

    void Start()
    {
        Cursor.visible = false;
    }

    public void SetNormalCursor() => SetCursor(normalCursor, Vector2.zero);
    public void SetPointCursor() => SetCursor(pointCursor, Vector2.zero);

    void Update()
    {
        cursor.rectTransform.position = Mouse.current.position.ReadValue();
    }

    public void SetCursor(Sprite cursorTexture, Vector2 hotspot)
    {
        cursor.sprite = cursorTexture;
        
        /*#if UNITY_WEBGL && !UNITY_EDITOR
            Cursor.SetCursor(cursorTexture, hotspot, CursorMode.ForceSoftware);
        #else
            Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
        #endif*/
    }
}