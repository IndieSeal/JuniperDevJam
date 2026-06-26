using Sirenix.OdinInspector;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private BoxCollider2D killBox;
    [SerializeField] private Spinner spinner;

    [Header("Activation")]
    [SerializeField] private bool percentageBasedMovement = false;
    [SerializeField] private float activationThreshold = 0.8f;
    [SerializeField] private bool reverseKillBox;

    [Header("Move")]
    [SerializeField] private bool useGlobalPosition = true;
    [SerializeField] private float gateMoveSpeed = 4;
    [SerializeField] private Vector2 maxExtraPosition = Vector2.up * 3;
    private Vector2 originalPosition;

    protected Vector2 Position
    {
        get => useGlobalPosition ? transform.position : transform.localPosition;
        set
        {
            if(useGlobalPosition) transform.position = value;
            else transform.localPosition = value;
        }
    }

    void Awake()
    {        
        originalPosition = Position;
    }

    void OnEnable()
    {
        GameManager.OnResetLevel += ResetLevel;
    }

    void OnDisable()
    {
        GameManager.OnResetLevel -= ResetLevel;
    }

    void Update()
    {
        Vector2 targetPosition = originalPosition;
        if(spinner.GetProgress() >= activationThreshold)
        {
            targetPosition = originalPosition + maxExtraPosition; 
            bool state = false;
            if(reverseKillBox) state = !state;
            SetKillBoxState(state);
        }
        else
        {
            bool state = true;
            if(reverseKillBox) state = !state;
            SetKillBoxState(state);
        }

        if(percentageBasedMovement) targetPosition = Vector2.Lerp(originalPosition, originalPosition + maxExtraPosition, spinner.GetProgress());
        Position = Vector2.Lerp(Position, targetPosition, gateMoveSpeed * Time.deltaTime);
    }
    private void SetKillBoxState(bool state)
    {
        if(killBox != null) killBox.enabled = state;
    }

    private void ResetLevel()
    {
        Position = originalPosition;
    }
}