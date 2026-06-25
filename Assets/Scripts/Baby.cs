using UnityEngine;

public class Baby : Priest
{
    [SerializeField] private GameObject happyBabyParticles;
    [SerializeField] private Animator animator;

    protected override void Update()
    {
        base.Update();

        if(GetProgress() < 1 && isWithinRange) animator.SetTrigger("Mad");
    }

    protected override void OnFinish()
    {
        base.OnFinish();

        animator.SetTrigger("Happy");
        happyBabyParticles.SetActive(true);
    }

    protected override void OnLevelReset()
    {
        base.OnLevelReset();

        animator.SetTrigger("Normal");
        happyBabyParticles.SetActive(false);
    }
}