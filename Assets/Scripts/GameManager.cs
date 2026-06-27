using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static event Action OnResetLevel;

    [Header("Settings Menu")]
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private Button levelSelectBtn;
    [SerializeField] private Button restartEntirely;

    [MinValue(0.5f), MaxValue(3f), SerializeField, DisableInEditorMode, OnValueChanged("ChangeTimeScale")] private float timeScale = 1;

    [Header("Components")]
    [SerializeField] private AudioSource audioSource;

    [FoldoutGroup("Audio"), SerializeField] private AudioClip winSound;
    [FoldoutGroup("Audio"), SerializeField] private AudioClip deathSound;

    //Requirements for reset
    private bool cameraReached;
    
    void Awake()
    {
        SpinnerCollider.IsGlobalInteractable = true;
        settingsMenu.SetActive(false);
    }

    void OnEnable()
    {
        Vampire.OnDeath += OnVampireDeath;
        Vampire.OnWin += OnVampireWin;

        CameraMovement.CameraReachedDeathDestination += OnCameraReachDestination;

        levelSelectBtn.onClick.AddListener(GoToLevelSelect);
        restartEntirely.onClick.AddListener(RestartEntirely);
    }

    void OnDisable()
    {
        Vampire.OnDeath -= OnVampireDeath;
        Vampire.OnWin -= OnVampireWin;

        CameraMovement.CameraReachedDeathDestination -= OnCameraReachDestination;

        levelSelectBtn.onClick.RemoveListener(GoToLevelSelect);
        restartEntirely.onClick.RemoveListener(RestartEntirely);
    }

    private void OnCameraReachDestination() => cameraReached = true;

    private void GoToLevelSelect() => StartCoroutine(TransitionManager.Instance.TransitionToNewScene("LevelSelect"));
    private void RestartEntirely() => StartCoroutine(TransitionManager.Instance.TransitionToNewScene(SceneManager.GetActiveScene().name));

    void Update()
    {
        if(Keyboard.current.rKey.wasPressedThisFrame) settingsMenu.SetActive(!settingsMenu.activeSelf);
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
        GoToLevelSelect();
        audioSource.volume = .3f;
        audioSource.PlayOneShot(winSound);
    }

    private void OnVampireDeath(Vampire vamp) => StartCoroutine(DeathScreen());

    private IEnumerator DeathScreen()
    {
        SpinnerCollider.IsGlobalInteractable = false;

        while(!cameraReached) yield return null;
        
        audioSource.PlayOneShot(deathSound);
        
        yield return ResetLevel();
        
        //Reset requirements
        cameraReached = false;
    }

    private IEnumerator ResetLevel()
    {
        yield return TransitionManager.Instance.Transition();

        SpinnerCollider.IsGlobalInteractable = true;
        OnResetLevel?.Invoke();
    }
}