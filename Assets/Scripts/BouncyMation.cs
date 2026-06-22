using UnityEngine;

public class BouncyMation : MonoBehaviour
{
    [Header("Facing")]
    [SerializeField] private Transform facingRoot;
    [SerializeField] private float flipSmooth = 15f;

    [Header("Juice")]
    [SerializeField] private Transform visual;
    [SerializeField] private float bounceHeight = 0.15f;
    [SerializeField] private float bounceSpeed = 10f;
    [SerializeField] private float tiltAmount = 12f;
    [SerializeField] private float tiltSmooth = 12f;
    [SerializeField] private float squashAmount = 0.08f;
    private Vector3 visualStartLocalPos;
    private Vector3 visualStartScale;
    private float bounceTimer;

    private float targetYRotation;

    private void Awake()
    {
        visualStartLocalPos = visual.localPosition;
        visualStartScale = visual.localScale;
    }

    public void Handle(Vector2 input)
    {
        UpdateFacing(input);
        AnimateSprite(input);
    }
    
    private void UpdateFacing(Vector2 input)
    {
        if (input.x > 0.01f) targetYRotation = 0f;
        else if (input.x < -0.01f) targetYRotation = 180f;

        Quaternion targetRotation = Quaternion.Euler(0f, targetYRotation, 0f);
        facingRoot.localRotation = Quaternion.Lerp(facingRoot.localRotation, targetRotation, flipSmooth * Time.deltaTime);
    }

    private void AnimateSprite(Vector2 input)
    {
        bool isMoving = input.sqrMagnitude > 0.01f;

        if (isMoving)
        {
            bounceTimer += Time.deltaTime * bounceSpeed;

            float bounce = Mathf.Abs(Mathf.Sin(bounceTimer)) * bounceHeight;
            visual.localPosition = visualStartLocalPos + Vector3.up * bounce;

            float targetTilt = tiltAmount * input.y;
            visual.localRotation = Quaternion.Lerp(visual.localRotation, Quaternion.Euler(0f, 0f, targetTilt), tiltSmooth * Time.deltaTime);

            float squash = Mathf.Sin(bounceTimer) * squashAmount;
            visual.localScale = new Vector3(visualStartScale.x + squash, visualStartScale.y - squash, visualStartScale.z);
        }
        else
        {
            bounceTimer = 0f;

            visual.localPosition = Vector3.Lerp(visual.localPosition, visualStartLocalPos,10f * Time.deltaTime);
            visual.localRotation = Quaternion.Lerp(visual.localRotation, Quaternion.identity, tiltSmooth * Time.deltaTime);
            visual.localScale = Vector3.Lerp(visual.localScale, visualStartScale, 10f * Time.deltaTime);
        }
    }
}