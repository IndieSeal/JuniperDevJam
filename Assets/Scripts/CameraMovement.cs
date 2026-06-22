using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
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

    private Camera myCam;

    void Awake()
    {
        myCam = GetComponent<Camera>();
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
        transform.position += (Vector3)(input * moveSpeed * Time.deltaTime);
    }

    private void HandleMouseZoom()
    {
        float scroll = Mouse.current.scroll.y.ReadValue();
        if (scroll != 0) myCam.orthographicSize -= scroll * zoomSpeed;

        myCam.orthographicSize = Mathf.Clamp(myCam.orthographicSize, minZoom, maxZoom);
    }
}