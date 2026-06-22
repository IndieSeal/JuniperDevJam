using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 1f;
    [SerializeField] private float minZoom = 10f;
    [SerializeField] private float maxZoom = 20f;

    private CinemachineConfiner2D bounds;
    private CinemachineCamera myCam;

    void Awake()
    {
        GameManager.OnResetLevel += ResetLevel;
        
        myCam = GetComponent<CinemachineCamera>();
        bounds = myCam.GetComponent<CinemachineConfiner2D>();
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
    }

    private void Update()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        Move(input);
        HandleMouseZoom();
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
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, min.x, max.x), Mathf.Clamp(transform.position.y, min.y, max.y), transform.position.z);
    }

    private void HandleMouseZoom()
    {
        float scroll = Mouse.current.scroll.y.ReadValue();
        if (scroll != 0) myCam.Lens.OrthographicSize -= scroll * zoomSpeed;

        myCam.Lens.OrthographicSize = Mathf.Clamp(myCam.Lens.OrthographicSize, minZoom, maxZoom);
        bounds.InvalidateLensCache();
    }

    private void ResetLevel()
    {
        
    }
}