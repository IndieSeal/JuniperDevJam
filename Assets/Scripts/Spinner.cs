using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Spinner : MonoBehaviour
{
    [SerializeField] private SpinnerCollider spinnerCollider;
    protected bool isSelected;

    protected virtual void Awake()
    {
        spinnerCollider.OnPointerDown += OnPointerDown;
        spinnerCollider.OnPointerUp += OnPointerUp;

        spinnerCollider.OnPointerDown += () => isSelected = true;
        spinnerCollider.OnPointerUp += () => isSelected = false;
    }

    public virtual void OnPointerDown() => Debug.Log("hello, thx for clicking me");
    public virtual void OnPointerUp() => Debug.Log("hello, hate you for leaving me");
    public abstract float GetProgress();
}