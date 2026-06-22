using UnityEngine;

public class Animal : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private BouncyMation bouncyMation;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    
    [Header("Death")]
    [SerializeField] private float pushForce = 5;
    private bool dead;
    
    void OnMouseDown()
    {
        Kill(Utilities.Get2DMouseWorldPosition());
    }

    public void Kill(Vector3 hitPosition)
    {
        if(dead) return;
        
        Vector2 direction = (hitPosition - transform.position).normalized * -1;
        rb.AddForce(direction * pushForce, ForceMode2D.Impulse);

        animator.SetTrigger("Kill");

        dead = true;
    }
}