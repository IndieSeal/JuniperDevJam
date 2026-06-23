using System;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Spinner : MonoBehaviour
{
    public static event Action<Spinner> OnSelected;
    public static event Action<Spinner> OnUnselected;
    
    [SerializeField] private SpinnerCollider spinnerCollider;
    public bool IsSelected { get; private set; }

    protected virtual void Awake()
    {
        spinnerCollider.OnPointerDown += PointerDown;
        spinnerCollider.OnPointerUp += PointerUp;
    }

    public void PointerDown()
    {
        OnPointerDown();
        IsSelected = true;
        OnSelected?.Invoke(this);
    }
    public void PointerUp()
    {
        if(!IsSelected) return;
        
        OnPointerUp();
        IsSelected = false;
        OnUnselected?.Invoke(this);
    }

    protected virtual void OnPointerDown() {}
    protected virtual void OnPointerUp() {}
    public abstract float GetProgress();
}