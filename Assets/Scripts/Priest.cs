using UnityEngine;

public class Priest : SpinMechanic
{
    [SerializeField] private ParticleSystem waterParticles;
    private bool canAttack = true;

    [SerializeField] private float priestRotationRange = 4;
    private Vector2 grabOriginalPosition;
    private Vector2 initialGrabOffset;

    protected override void UpdateVisuals()
    {
        Vector2 newPosition = Utilities.Get2DMouseWorldPosition() - initialGrabOffset;
        transform.position = Vector2.ClampMagnitude(newPosition - grabOriginalPosition, priestRotationRange) + grabOriginalPosition;
    }

    protected override void HandleSpin()
    {
        base.HandleSpin();

        Instantiate(waterParticles, waterParticles.transform.position, Quaternion.identity).gameObject.SetActive(true);
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