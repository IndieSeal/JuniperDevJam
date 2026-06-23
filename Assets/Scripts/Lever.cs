using DrawXXL;
using UnityEngine;
using UnityEngine.EventSystems;

public class Lever : Spinner
{
    [Header("Limiters")]
    [SerializeField] private float minAngle = -45;
    [SerializeField] private float maxAngle = 45;
    
    [SerializeField] private float rotationSpeed = 5;
    [SerializeField] private Transform rotationHandle;
    [SerializeField] private bool canReturnToOriginal = true;
    private float targetAngle;
    public bool State { get; private set; } // Off = false, On = true :D!

    protected override void Awake()
    {
        base.Awake();

        GameManager.OnResetLevel += ResetLevel;
    }

    void Start()
    {
        ResetLevel();
    }

    private void ResetLevel()
    {
        SetZEulerAngle(maxAngle);
        targetAngle = maxAngle;
        State = false;
    }

    void Update()
    {        
        targetAngle = Mathf.DeltaAngle(0, targetAngle);
        targetAngle = Mathf.Clamp(targetAngle, minAngle, maxAngle);

        float currentAngle = Mathf.DeltaAngle(0, rotationHandle.eulerAngles.z);
        float angle = Mathf.Lerp(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
        SetZEulerAngle(angle);

        SetState(GetProgress());

        float howClose = GetLocalProgress();
        if (!IsSelected)
        {
            if(howClose < 0.4f) targetAngle = minAngle;
            else if(howClose > 0.6f) targetAngle = maxAngle;
            return;
        }
        
        if(!IsMouseAboveLever() || (!canReturnToOriginal && State)) return;

        targetAngle = Utilities.LookAt2DAngle(Utilities.Get2DMouseWorldPosition(), rotationHandle.position, 90);
    }

    private void SetZEulerAngle(float value) => rotationHandle.eulerAngles = new Vector3(rotationHandle.eulerAngles.x, rotationHandle.eulerAngles.y, value);
    private void SetState(float closeness)
    {
        if(closeness < 0.1f) State = false;
        else if(closeness > 0.9f) State = true;
    }

    private bool IsMouseAboveLever()
    {
        Vector2 leverUp = rotationHandle.up;
        Vector2 leverToMouse = (Utilities.Get2DMouseWorldPosition() - (Vector2)rotationHandle.position).normalized;

        return Vector2.Dot(leverUp, leverToMouse) > 0;
    }

    private float GetLocalProgress()
    {
        float angle = Mathf.DeltaAngle(0, rotationHandle.eulerAngles.z);
        return Mathf.InverseLerp(minAngle, maxAngle, angle);;
    }

    public override float GetProgress()
    {
        return 1 - GetLocalProgress();
    }

    void OnDrawGizmosSelected()
    {
        DrawBasics2D.CircleSegment(transform.position, minAngle, maxAngle, 1);
    }
}