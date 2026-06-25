using System;
using UnityEngine;

public class ColliderArea : MonoBehaviour
{
    public event Action<GameObject> OnCollisionEnter;
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}