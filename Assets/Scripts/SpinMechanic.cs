using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public enum SpinDirection
{
    Right,
    Left,
    Both
}

public class SpinMechanic : Spinner
{
    [SerializeField] private Transform rotatingTransform;
    [SerializeField] private SpinDirection lockedDirection = SpinDirection.Right;
    [Space]
    [SerializeField, Tooltip("If it's less or equal to 0, then you can rotate infinitely")] private int maxRotations = 5;
    [ReadOnly, SerializeField] private int rotationCounter;

    private Vector2 previousDirection;
    private Vector2 startMousePosition;

    private float accumulatedRotation;
    private float visualRotation;

    protected override void Awake()
    {
        base.Awake();

        GameManager.OnResetLevel += ResetLevel;
    }

    private void ResetLevel()
    {
        accumulatedRotation = 0;
        visualRotation = 0;

        rotationCounter = 0;

        previousDirection = Vector2.zero;
        startMousePosition = Vector2.zero;

        SetZRotation(visualRotation);
    }

    protected override void OnPointerDown()
    {
        previousDirection = Vector2.zero;
        startMousePosition = Utilities.Get2DMouseWorldPosition();
    }

    private void Update()
    {
        if(!IsSelected) return;
        
        Vector2 mousePosition = Utilities.Get2DMouseWorldPosition();

        Vector2 centerPosition = startMousePosition;
        if(rotatingTransform != null) centerPosition = rotatingTransform.position;
        Vector2 currentDirection = (mousePosition - centerPosition).normalized;

        bool canRotate = maxRotations <= 0 || rotationCounter < maxRotations;
        if (previousDirection != Vector2.zero && canRotate)
        {            
            bool validDirection = true;
            
            float delta = Vector2.SignedAngle(previousDirection, currentDirection);
            if(lockedDirection != SpinDirection.Both && ((lockedDirection == SpinDirection.Left && delta > 0) || (lockedDirection == SpinDirection.Right && delta < 0))) validDirection = false;

            float maxAnglePerAccumulation = 40; // So players can't just grab the center point and start doing the fastest spins of their lives
            if (validDirection && Mathf.Abs(delta) < maxAnglePerAccumulation)
            {
                accumulatedRotation += delta;
                visualRotation += delta;
                if (maxRotations > 0)
                {
                    float limit = maxRotations * 360f;
                    switch (lockedDirection)
                    {
                        case SpinDirection.Right:
                            Mathf.Clamp(visualRotation, 0, limit);
                            break;
                        case SpinDirection.Left:
                            Mathf.Clamp(visualRotation, -limit, 0);
                            break;
                        default:
                            Mathf.Clamp(visualRotation, -limit, limit);
                            break;
                    }
                }
                
                SetZRotation(visualRotation);

                float rotation = 360; // How much you got to spin for a spin to count :D
                if (Mathf.Abs(accumulatedRotation) >= rotation) HandleSpin();
            }    
        }

        previousDirection = currentDirection;
    }

    private void SetZRotation(float value)
    {
        if(rotatingTransform != null) rotatingTransform.eulerAngles = new Vector3(rotatingTransform.eulerAngles.x, rotatingTransform.eulerAngles.y, value);
    }

    private void HandleSpin()
    {
        rotationCounter++;
        accumulatedRotation -= Mathf.Sign(accumulatedRotation) * 360;
    }

    private float GetLimitedProgress()
    {
        float maxProgress = maxRotations * 360f;
        float totalProgress = rotationCounter * 360f + Mathf.Abs(accumulatedRotation);

        return Mathf.Clamp01(totalProgress / maxProgress);
    }

    public override float GetProgress()
    {
        if(maxRotations <= 0) return Mathf.InverseLerp(0, 360, Mathf.Abs(accumulatedRotation));
        return GetLimitedProgress();
    }
}