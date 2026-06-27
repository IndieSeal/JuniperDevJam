using System;
using UnityEngine;

public class Baby : Priest
{
    public event Action OnTakenCare;
    public event Action OnAttack;
    
    [SerializeField] private GameObject explodeGuardsParticles;
    
    [SerializeField] private GameObject happyBabyParticles;
    [SerializeField] private AudioClip cryingBaby;
    [SerializeField] private AudioClip madBaby;
    [SerializeField] private AudioClip happyBaby;
    [SerializeField, Tooltip("In this case, for tutorial use")] private bool waitForInstructions = false;
    private bool carryOnDefaultBehaviour;

    protected override void Update()
    {
        if(waitForInstructions && !carryOnDefaultBehaviour) return;
        
        base.Update();

        audioSource.spatialBlend = IsSelected ? 0 : 1;
    }

    public void InstructionAlert()
    {
        Alert();
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

    public void InstructionAttack()
    {
        Attack(null);
    }

    public void SetBabyNormality(bool state)
    {
        carryOnDefaultBehaviour = state;
    }

    protected override void Attack(Vampire vamp)
    {
        base.Attack(vamp);
        animator.SetTrigger("Mad");

        OnAttack?.Invoke();

        audioSource.Stop();
        audioSource.loop = true;
        audioSource.clip = madBaby;
        audioSource.Play();
    }

    protected override void OnFinish()
    {
        OnTakenCare?.Invoke();

        animator.SetTrigger("Happy");
        happyBabyParticles.SetActive(true);

        audioSource.Stop();
        audioSource.PlayOneShot(happyBaby);

        Debug.Log("Finished");
        foreach(GameObject go in instancesOfAttacks)
        {
            Instantiate(explodeGuardsParticles, go.transform.position, Quaternion.identity);
            Destroy(go);
            Debug.Log("Should destroy");
        }
        instancesOfAttacks.Clear();

        base.OnFinish();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        HandleCollision(collision.gameObject);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        HandleCollision(collision.gameObject);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        HandleCollision(collision.gameObject);
    }

    private void HandleCollision(GameObject go)
    {
        Debug.Log(go.tag);
        if(go.CompareTag("KillBox") && go.TryGetComponent(out SpinnerCollider spinnerCollider) && spinnerCollider.canKillGuards)
        {
            //Instantiate(explodeGuardsParticles, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }
    }

    protected override void OnLevelReset()
    {
        base.OnLevelReset();

        animator.SetTrigger("Happy");
        happyBabyParticles.SetActive(false);
        gameObject.SetActive(true);

        audioSource.Stop();
    }
}