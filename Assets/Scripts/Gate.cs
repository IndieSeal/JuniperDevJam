using UnityEngine;

public class Gate : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private BoxCollider2D killBox;
    [SerializeField] private Spinner spinner;

    [Header("Move")]
    [SerializeField] private float gateMoveSpeed = 4;
    [SerializeField] private Vector2 maxExtraPosition = Vector2.up * 3;
    private Vector2 originalPosition;

    void Awake()
    {
        originalPosition = transform.position;

        GameManager.OnResetLevel += ResetLevel;
        ResetLevel();
    }

    void Update()
    {
        Vector2 targetPosition = originalPosition;
        if(spinner.GetProgress() >= 0.8f)
        {
            targetPosition = originalPosition + maxExtraPosition; 
            killBox.enabled = false;
        }
        else
        {
            killBox.enabled = true;
        }

        transform.position = Vector2.Lerp(transform.position, targetPosition, gateMoveSpeed * Time.deltaTime);
    }

    private void ResetLevel()
    {
        transform.position = originalPosition;
    }
}