using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static event Action OnResetLevel;

    [MinValue(0.5f), MaxValue(3f), SerializeField, DisableInEditorMode, OnValueChanged("ChangeTimeScale")] private float timeScale = 1;

    [Header("Components")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Animator animator;

    [FoldoutGroup("Audio"), SerializeField] private AudioClip winSound;
    [FoldoutGroup("Audio"), SerializeField] private AudioClip deathSound;

    //Requirements for reset
    private bool cameraReached;
    
    void Awake()
    {
        Vampire.OnDeath += OnVampireDeath;
        Vampire.OnWin += OnVampireWin;

        CameraMovement.CameraReachedDeathDestination += () => cameraReached = true;
    }

    private void ChangeTimeScale()
    {
        if(!Application.isPlaying) return;

        Time.timeScale = timeScale;
    }

    private void OnVampireWin(Vampire vamp)
    {
        
    }

    private void OnVampireDeath(Vampire vamp) => StartCoroutine(DeathScreen());

    private IEnumerator DeathScreen()
    {
        while(!cameraReached) yield return null;
        
        audioSource.PlayOneShot(deathSound);
        
        animator.SetTrigger("Lose");
        yield return null;

        float clipDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(clipDuration);

        //while(!Keyboard.current.rKey.wasPressedThisFrame) yield return null;
        animator.SetTrigger("Reset");
        OnResetLevel?.Invoke();
        
        //Reset requirements
        cameraReached = false;
    }
}