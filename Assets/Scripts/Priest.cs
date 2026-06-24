using UnityEngine;

public class Priest : SpinMechanic
{
    [SerializeField] private ParticleSystem waterParticles;
    private bool canAttack = true;

    [SerializeField] private Vector2 grabOffset;
    [SerializeField] private float priestRotationRange = 4;
    private Vector2 grabOriginalPosition;

    protected override void UpdateVisuals()
    {
        Vector2 newPosition = Utilities.Get2DMouseWorldPosition() + grabOffset;
        Vector2 center = grabOriginalPosition;
        transform.position = Vector2.ClampMagnitude(newPosition - center, priestRotationRange) + center;
    }

    protected override void HandleSpin()
    {
        base.HandleSpin();

        waterParticles.Play();
    }

    protected override void OnPointerDown()
    {
        grabOriginalPosition = transform.position;
        base.OnPointerDown();
    }

    protected override void OnPointerUp()
    {
        transform.position = grabOriginalPosition;
        base.OnPointerUp();
    }
}