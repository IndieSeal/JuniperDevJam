using System.Collections.Generic;
using UnityEngine;

public class WaterGate : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private BoxCollider2D killBox;
    [SerializeField] private Animator animator;
    [SerializeField] private List<Spinner> spinners = new List<Spinner>();
    [SerializeField] private bool isKillBoxReverse;
    private bool isBroken;

    void Awake()
    {
        ResetLevel();
    }

    void OnEnable()
    {
        GameManager.OnResetLevel += ResetLevel;
    }

    void OnDisable()
    {
        GameManager.OnResetLevel -= ResetLevel;
    }

    void Update()
    {
        if(isBroken) return;

        bool hasBrokenAll = true;

        foreach(var spinner in spinners)
        {
            if(spinner.GetProgress() != 1)
            {
                hasBrokenAll = false;
                break;
            }
        }

        bool enabledState = isKillBoxReverse;
        if(hasBrokenAll)
        {
            Debug.Log("Play Animation");
            killBox.enabled = enabledState;
            animator.SetTrigger("Break");
            isBroken = true;
        }
        else killBox.enabled = !enabledState;
    }

    private void ResetLevel()
    {
        animator.SetTrigger("Reset");
        isBroken = false;
    }
}