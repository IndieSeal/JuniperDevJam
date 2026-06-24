using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : Singleton<CameraMovement>
{
    public static event Action CameraReachedDeathDestination;
    
    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    private Vector2 MoveInput => moveAction.action.ReadValue<Vector2>();

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 17.5f;
    private Vector2 originalPosition;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 1f;
    [SerializeField] private float minZoom = 10f;
    [SerializeField] private float maxZoom = 20f;
    private float originalZoom;
    private CinemachineConfiner2D bounds;
    private CinemachineCamera myCam;

    // Death screen
    [Header("Death Screen")]
    [SerializeField] private float deathClosenessMoveDuration = 0.5f;
    [Space]
    [SerializeField] private float deathClosenessZoomSpeed = 1.2f;
    [SerializeField] private float deathClosenessZoom = 5f;

    // Force move towards 
    private Vector2 closenessTargetPosition;
    private Vector2 closenessStartPosition;
    private float closenessTimer;
    private bool forceCloseness;
    private bool hasReachedFinalDestination;

    private float closenessMoveDuration;
    private float closenessZoomSpeed;
    private float closenessZoom;

    protected override void Awake()
    {
        base.Awake();
        
        myCam = GetComponent<CinemachineCamera>();
        bounds = myCam.GetComponent<CinemachineConfiner2D>();
        
        originalPosition = transform.position;
        originalZoom = myCam.Lens.OrthographicSize;
    }

    private void OnEnable()
    {
        moveAction.action.Enable();

        GameManager.OnResetLevel += ResetLevel;

        Vampire.OnWin += OnVampireWin;
        Vampire.OnDeath += OnVampireDeath;

        PathFollower.OnReachCheckpoint += ReachCheckpoint;
    }

    private void OnDisable()
    {
        moveAction.action.Disable();

        GameManager.OnResetLevel -= ResetLevel;

        Vampire.OnWin -= OnVampireWin;
        Vampire.OnDeath -= OnVampireDeath;

        PathFollower.OnReachCheckpoint -= ReachCheckpoint;
    }

    private void Update()
    {
        if (!forceCloseness)
        {
            Move(MoveInput);
            HandleMouseZoom();   
        }
        else
        {            
            closenessTimer += Time.deltaTime;
            MoveWithoutZ(Vector2.Lerp(closenessStartPosition, closenessTargetPosition, closenessTimer / closenessMoveDuration));
            myCam.Lens.OrthographicSize = Mathf.Lerp(myCam.Lens.OrthographicSize, closenessZoom, closenessZoomSpeed * Time.deltaTime);

            if(Vector2.Distance(transform.position, closenessTargetPosition) < 0.01f && !hasReachedFinalDestination)
            {
                CameraReachedDeathDestination?.Invoke();
                hasReachedFinalDestination = true;
            }
        }
    }

    private void OnVampireWin(Vampire vampire) => OnVampireDeath(vampire);
    private void OnVampireDeath(Vampire vampire) => ForceCamera(vampire.transform.position, deathClosenessMoveDuration, deathClosenessZoom, deathClosenessZoomSpeed, true);

    public void ForceCamera(Vector2 position, float moveDuration, float zoom, float zoomSpeed, bool resetCameraPosition = true)
    {
        forceCloseness = true;
        if(resetCameraPosition) hasReachedFinalDestination = false;
        
        closenessTargetPosition = position;
        bounds.enabled = false;
        
        closenessTimer = 0;
        closenessStartPosition = transform.position;

        closenessMoveDuration = moveDuration;
        closenessZoom = zoom;
        closenessZoomSpeed = zoomSpeed;
    }

    public void UnforceCamera()
    {
        forceCloseness = false;
        bounds.enabled = true;

        hasReachedFinalDestination = false;
    }

    private void Move(Vector2 input)
    {
        /*float extraSpeed = bounds.GetCameraDisplacementDistance(myCam) * 0.4f;
        Debug.Log(extraSpeed);
        extraSpeed = Mathf.Clamp(extraSpeed, 1, 3);
        */

        float extraSpeed = Keyboard.current.shiftKey.isPressed ? 2 : 1;

        transform.position += (Vector3)(input * extraSpeed * moveSpeed * Time.deltaTime);

        Vector2 min = bounds.BoundingShape2D.bounds.min;
        Vector2 max = bounds.BoundingShape2D.bounds.max;
        MoveWithoutZ(new Vector2(Mathf.Clamp(transform.position.x, min.x, max.x), Mathf.Clamp(transform.position.y, min.y, max.y)));
    }

    private void ReachCheckpoint(PathPoint pathPoint)
    {
        originalPosition = pathPoint.transform.position;
        originalZoom = pathPoint.zoom;
    }

    private void MoveWithoutZ(Vector2 newPosition) => transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);

    private void HandleMouseZoom()
    {
        float scroll = Mouse.current.scroll.y.ReadValue();
        if (scroll != 0) myCam.Lens.OrthographicSize -= scroll * zoomSpeed;

        myCam.Lens.OrthographicSize = Mathf.Clamp(myCam.Lens.OrthographicSize, minZoom, maxZoom);
        bounds.InvalidateLensCache();
    }

    private void ResetLevel()
    {
        UnforceCamera();

        MoveWithoutZ(originalPosition);
        myCam.Lens.OrthographicSize = originalZoom;
        bounds.InvalidateLensCache();
    }
}