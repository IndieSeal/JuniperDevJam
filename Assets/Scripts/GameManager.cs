using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

    [Button]
    private void OnVampireWin(Vampire vamp)
    {
        PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, 1);
        StartCoroutine(TransitionToNewScene("LevelSelect"));
    }

    private void OnVampireDeath(Vampire vamp) => StartCoroutine(DeathScreen());

    private IEnumerator DeathScreen()
    {
        while(!cameraReached) yield return null;
        
        audioSource.PlayOneShot(deathSound);
        
        animator.SetTrigger("Lose");
        
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Lose"));

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float clipDuration = stateInfo.length / animator.speed;
        yield return new WaitForSeconds(clipDuration);

        animator.SetTrigger("Reset");
        OnResetLevel?.Invoke();
        
        //Reset requirements
        cameraReached = false;
    }

    private IEnumerator TransitionToNewScene(string sceneName)
    {
        animator.SetTrigger("Lose");

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Lose"));

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float clipDuration = stateInfo.length / animator.speed;
        yield return new WaitForSeconds(clipDuration);

        animator.SetTrigger("Reset");

        animator.transform.SetParent(null);
        DontDestroyOnLoad(animator.gameObject);
        SceneManager.activeSceneChanged += (s1, s2) => Destroy(animator.gameObject, 5);
        
        SceneManager.LoadScene(sceneName);
    }
}