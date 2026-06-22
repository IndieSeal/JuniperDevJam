using System;
using UnityEngine;

public class Vampire : MonoBehaviour
{
    public static event Action<Vampire> OnDeath;
    private bool isDead;

    [Header("Components")]
    [SerializeField] private Animator animator;

    void Awake()
    {
        GameManager.OnResetLevel += ResetLevel;
    }

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
        if(!collision.CompareTag("KillBox") || isDead) return;

        OnDeath?.Invoke(this);
        animator.SetTrigger("Kill");
        isDead = true;
    }

    private void ResetLevel()
    {
        animator.SetTrigger("Reset");
        isDead = false;
    }
}