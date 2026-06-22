using UnityEngine;

public class Vampire : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleEnterCollision(collision.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        HandleEnterCollision(collision.gameObject);
    }

    private void HandleEnterCollision(GameObject collision)
    {
        TryKill(collision);        
    }

    private void TryKill(GameObject collision)
    {
        if(!collision.CompareTag("KillBox")) return;

        Destroy(gameObject);
    }
}