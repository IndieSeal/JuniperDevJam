using UnityEngine;

public class Priest : SpinMechanic
{
    [SerializeField] private ParticleSystem waterParticles;
    [SerializeField] private float priestParticleRange = 2;
    protected bool isWithinRange;
    protected bool canAttack = true;

    [SerializeField] private float priestRotationRange = 4;
    private Vector2 grabOriginalPosition;
    private Vector2 initialGrabOffset;

    protected override void UpdateVisuals()
    {
        transform.position = GetClampedPosition(Utilities.Get2DMouseWorldPosition());
    }

    protected override void HandleSpin()
    {
        base.HandleSpin();

        Vector2 position = grabOriginalPosition + (Vector2.up * 5) + (Random.insideUnitCircle * priestParticleRange);
        Instantiate(waterParticles, position, Quaternion.identity).gameObject.SetActive(true);
    }

    private Vector2 GetClampedPosition(Vector2 position)
    {
        Vector2 newPosition = position - initialGrabOffset;
        return Vector2.ClampMagnitude(newPosition - grabOriginalPosition, priestRotationRange) + grabOriginalPosition;
    }

    protected override void OnPointerDown()
    {
        grabOriginalPosition = transform.position;
        initialGrabOffset = (Vector3)Utilities.Get2DMouseWorldPosition() - transform.position;
        base.OnPointerDown();
    }

    protected override void OnPointerUp()
    {
        transform.position = grabOriginalPosition;
        base.OnPointerUp();
    }
}