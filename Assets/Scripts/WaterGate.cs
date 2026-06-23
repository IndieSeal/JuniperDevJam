using System.Collections.Generic;
using UnityEngine;

public class WaterGate : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private BoxCollider2D killBox;
    [SerializeField] private Animator animator;
    [SerializeField] private List<Spinner> spinners = new List<Spinner>();
    private bool isBroken;

    void Awake()
    {
        GameManager.OnResetLevel += ResetLevel;
        ResetLevel();
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

        if(hasBrokenAll)
        {
            killBox.enabled = false;
            animator.SetTrigger("Break");
            isBroken = true;
        }
        else killBox.enabled = true;
    }

    private void ResetLevel()
    {
        animator.SetTrigger("Reset");
        isBroken = false;
    }
}