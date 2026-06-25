using UnityEngine;

public class Baby : Priest
{
    [SerializeField] private GameObject happyBabyParticles;
    [SerializeField] private AudioClip cryingBaby;
    [SerializeField] private AudioClip madBaby;
    [SerializeField] private AudioClip happyBaby;

    protected override void Update()
    {
        base.Update();

        audioSource.spatialBlend = IsSelected ? 0 : 1;
    }

    protected override void Alert()
    {
        base.Alert();
        animator.SetTrigger("Normal");

        audioSource.Stop();
        audioSource.loop = true;
        audioSource.clip = cryingBaby;
        audioSource.Play();
    }

    protected override void Attack(Vampire vamp)
    {
        base.Attack(vamp);
        animator.SetTrigger("Mad");

        audioSource.Stop();
        audioSource.loop = true;
        audioSource.clip = madBaby;
        audioSource.Play();
    }

    protected override void OnFinish()
    {
        base.OnFinish();

        animator.SetTrigger("Happy");
        happyBabyParticles.SetActive(true);

        audioSource.Stop();
        audioSource.PlayOneShot(happyBaby);
    }

    protected override void OnLevelReset()
    {
        base.OnLevelReset();

        animator.SetTrigger("Happy");
        happyBabyParticles.SetActive(false);

        audioSource.Stop();
    }
}