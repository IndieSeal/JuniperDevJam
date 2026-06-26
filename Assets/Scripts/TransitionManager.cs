using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : Singleton<TransitionManager>
{
    [SerializeField] private Animator animator;

    protected override void Awake()
    {
        base.Awake();

        if(Instance == this)
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
    }

    public IEnumerator TransitionToNewScene(string sceneName)
    {
        yield return Transition(false);
        
        yield return null;

        this.animator.SetTrigger("Reset");
        SceneManager.LoadScene(sceneName);

        yield return null;

        yield return WaitForAnimation("Reset");
    }

    public IEnumerator Transition(bool autoPlayReset = true, Action onTransitionShown = null)
    {
        this.animator.SetTrigger("Lose");
        yield return WaitForAnimation("Lose");

        onTransitionShown?.Invoke();
        if(autoPlayReset) this.animator.SetTrigger("Reset");
    }

    public IEnumerator WaitForAnimation(string animName)
    {
        while(!this.animator.GetCurrentAnimatorStateInfo(0).IsName(animName)) yield return null;

        var stateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
        float clipDuration = stateInfo.length / this.animator.speed;
        yield return new WaitForSeconds(clipDuration);
    }
}