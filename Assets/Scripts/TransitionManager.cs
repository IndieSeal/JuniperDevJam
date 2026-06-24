using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : Singleton<TransitionManager>
{
    [SerializeField] private Animator animator;

    public IEnumerator TransitionToNewScene(string sceneName)
    {
        yield return Transition();
        
        DontDestroyOnLoad(gameObject);

        yield return null;

        SceneManager.LoadScene(sceneName);

        yield return WaitForAnimation("Reset");
        
        Destroy(gameObject);
    }

    public IEnumerator Transition(Action onTransitionShown = null)
    {
        animator.SetTrigger("Lose");
        yield return WaitForAnimation("Lose");

        onTransitionShown?.Invoke();
        animator.SetTrigger("Reset");
    }

    public IEnumerator WaitForAnimation(string animName)
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(animName));

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float clipDuration = stateInfo.length / animator.speed;
        yield return new WaitForSeconds(clipDuration);
    }
}