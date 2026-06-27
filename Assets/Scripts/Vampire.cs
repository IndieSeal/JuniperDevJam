using System;
using System.Collections;
using UnityEngine;

public class Vampire : MonoBehaviour
{
    public static event Action<Vampire> OnWin;
    public static event Action<Vampire> OnDeath;
    private bool isDead;

    [Header("Components")]
    [SerializeField] private Animator animator;

    void OnEnable()
    {
        GameManager.OnResetLevel += ResetLevel;
    }
    
    void OnDisable()
    {
        GameManager.OnResetLevel -= ResetLevel;
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
        TryWin(collision);    
    }

    private void TryWin(GameObject collision)
    {
        if(!collision.CompareTag("Win") || isDead) return;

        OnWin?.Invoke(this);
    }

    private void TryKill(GameObject collision)
    {
        if(!collision.CompareTag("KillBox") || isDead) return;

        GetComponent<Collider2D>().enabled = false;
        OnDeath?.Invoke(this);
        animator.SetTrigger("Kill");
        isDead = true;
    }

    private void ResetLevel()
    {
        GetComponent<Collider2D>().enabled = true;
        animator.SetTrigger("Reset");
        StartCoroutine(DeadSet());
        //isDead = false;
    }

    private IEnumerator DeadSet()
    {
        yield return null;
        isDead = false;
    }
}