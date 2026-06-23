using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpinnerCollider : MonoBehaviour
{
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
        highlightMaterial = true;
    }
    void OnMouseExit()
    {
        if(isPressing) return;
        highlightMaterial = false;
    }

    private void OnMouseDown()
    {
        OnPointerDown?.Invoke();
        highlightMaterial = true;
        isPressing = true;
    }
    private void OnMouseUp()
    {
        OnPointerUp?.Invoke();
        highlightMaterial = false;
        isPressing = false;
    }
}