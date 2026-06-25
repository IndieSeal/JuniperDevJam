using System;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Spinner : MonoBehaviour
{    
    public static event Action<Spinner> OnSelected;
    public static event Action<Spinner> OnUnselected;
    
    [SerializeField] private SpinnerCollider spinnerCollider;
    [SerializeField, Tooltip("When it hits 1, can it be altered?")] private bool setAndDone = true;
    public bool IsSelected { get; private set; }

    public bool IsFinished => GetProgress() >= 1 && setAndDone;

    protected virtual void OnEnable()
    {
        spinnerCollider.OnPointerDown += PointerDown;
        spinnerCollider.OnPointerUp += PointerUp;

        GameManager.OnResetLevel += OnLevelReset;
    }

    protected virtual void OnDisable()
    {
        spinnerCollider.OnPointerDown -= PointerDown;
        spinnerCollider.OnPointerUp -= PointerUp;

        GameManager.OnResetLevel -= OnLevelReset;
    }

    void LateUpdate()
    {
        if(IsSelected && IsFinished)
        {
            PointerUp();
            spinnerCollider.OnFinish();

            OnFinish();
        }
    }

    public void PointerDown()
    {
        if(IsFinished) return;
        
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

    protected virtual void OnFinish() {}

    protected virtual void OnPointerDown() {}
    protected virtual void OnPointerUp() {}
    public abstract float GetProgress();

    protected abstract void OnLevelReset();
}