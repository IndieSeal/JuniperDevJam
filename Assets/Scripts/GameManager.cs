using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static event Action OnResetLevel;
    
    void Awake()
    {
        Vampire.OnDeath += OnVampireDeath;
    }

    private void OnVampireDeath(Vampire vamp) => StartCoroutine(TimeBeforeAvailableReset());

    private IEnumerator TimeBeforeAvailableReset()
    {
        yield return new WaitForSeconds(1);
        while(!Keyboard.current.rKey.wasPressedThisFrame) yield return null;
        
        OnResetLevel?.Invoke();
    }
}