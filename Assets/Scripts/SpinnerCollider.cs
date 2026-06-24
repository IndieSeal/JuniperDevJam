using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpinnerCollider : MonoBehaviour
{
    public static bool IsInteractable { get; set; } = true;
    
    public event Action OnPointerDown;
    public event Action OnPointerUp;

    [InfoBox("OPTIONAL")]
    [SerializeField] private Renderer spriteRenderer;
    private bool highlightMaterial;
    private bool isPressing;

    void Update()
    {
        if(spriteRenderer == null) return;

        if(highlightMaterial) spriteRenderer.material.EnableKeyword("OUTBASE_ON");
        else spriteRenderer.material.DisableKeyword("OUTBASE_ON");
    }

    void OnMouseEnter()
    {
        if(!IsInteractable) return;
        
        CursorManager.Instance.SetPointCursor();
        highlightMaterial = true;
    }
    void OnMouseExit()
    {
        if(!IsInteractable) return;

        if(isPressing) return;
        highlightMaterial = false;

        CursorManager.Instance.SetNormalCursor();
    }

    private void OnMouseDown()
    {
        if(!IsInteractable) return;

        OnPointerDown?.Invoke();
        highlightMaterial = true;
        isPressing = true;
    }
    private void OnMouseUp()
    {
        if(!IsInteractable) return;

        OnPointerUp?.Invoke();
        highlightMaterial = false;
        isPressing = false;

        CursorManager.Instance.SetNormalCursor();
    }
}