using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Priest : SpinMechanic
{
    [Header("Vampire Find")]
    [SerializeField] private float findRange = 8;
    [SerializeField] private float foundRange = 15;
    [SerializeField] private float startAttackDelay = 0.6f;
    [SerializeField] private ParticleSystem alertParticle;

    [Header("Attack")]
    [SerializeField] private bool attackOnce = false;
    [SerializeField, HideIf("attackOnce")] private float repeatAttackDelay = 4;
    [Space]
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private float destroyTime = 20;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private bool goTowardsPlayer = true;
    [SerializeField, ShowIf("goTowardsPlayer")] private float velocity = 6;
    private bool hasAttacked;
    private float timer;
    protected List<GameObject> instancesOfAttacks = new List<GameObject>();
    
    [SerializeField] protected Animator animator;
    [SerializeField] protected AudioSource audioSource;
    
    [SerializeField] private ParticleSystem waterParticles;
    [SerializeField] private float priestParticleRange = 2;
    protected bool isWithinRange;

    [SerializeField] private float priestRotationRange = 4;
    private Vector2 grabOriginalPosition;
    private Vector2 initialGrabOffset;
    private Vector2 initialPosition;

    [Header("Priest Audio")]
    [SerializeField] private AudioSource madPerson;
    [SerializeField] private AudioClip shakeWater;

    [FoldoutGroup("Animation Names"), SerializeField] private string attackAnimation = "Attack";
    private bool playedAnimation;

    void Start()
    {
        initialPosition = transform.position;
    }

    protected override void Update()
    {
        base.Update();

        if(IsFinished) return;

        bool newWithinRange = false;
        Vampire vampire = null;
        float range = hasAttacked ? foundRange : findRange;
        foreach(Collider2D collider in Physics2D.OverlapCircleAll(transform.position, range))
        {
            if(collider.TryGetComponent(out vampire))
            {
                newWithinRange = true;
                break;
            }
        }

        if(isWithinRange != newWithinRange)
        {
            if(!newWithinRange) animator.SetTrigger("Reset");
        }
        isWithinRange = newWithinRange;
        
        if(!isWithinRange || (attackOnce && hasAttacked)) return;

        timer += Time.deltaTime;
        if(!playedAnimation) Alert();

        if(IsSelected) return;

        float delay = hasAttacked ? repeatAttackDelay : startAttackDelay;
        if(timer >= delay)
        {
            Attack(vampire);
            timer = 0;
            playedAnimation = false;
        }
    }

    protected virtual void Alert()
    {
        playedAnimation = true;
        alertParticle.Play();
        animator.SetTrigger("Alert");
    }

    protected virtual void Attack(Vampire vamp)
    {
        animator.SetTrigger(attackAnimation);

        var instance = Instantiate(attackPrefab, spawnPosition.position, Quaternion.identity);
        instancesOfAttacks.Add(instance);
        hasAttacked = true;

        //Vector2 direction =  (vamp.transform.position - instance.transform.position).normalized;
        if(goTowardsPlayer) StartCoroutine(MoveTowards(instance.transform, vamp.transform, destroyTime));
    }

    protected IEnumerator MoveTowards(Transform instance, Vector2 direction, float destroyTime = 20)
    {
        float timePassed = 0;
        while(timePassed < destroyTime)
        {
            instance.position += (Vector3)direction * Time.deltaTime * velocity;
            timePassed += Time.deltaTime;
            yield return null;
        }

        instancesOfAttacks.Remove(instance.gameObject);
        Destroy(instance);
    }

    protected IEnumerator MoveTowards(Transform instance, Transform direction, float destroyTime = 20)
    {
        float timePassed = 0;
        while(timePassed < destroyTime)
        {
            instance.position = Vector2.MoveTowards(instance.position, direction.position, velocity * Time.deltaTime);
            timePassed += Time.deltaTime;
            yield return null;
        }

        instancesOfAttacks.Remove(instance.gameObject);
        Destroy(instance);
    }

    protected override void OnLevelReset()
    {
        base.OnLevelReset();

        StopAllCoroutines();
        foreach(GameObject go in instancesOfAttacks) Destroy(go);
        instancesOfAttacks.Clear();

        hasAttacked = false;
        isWithinRange = false;
        timer = 0;

        transform.position = initialPosition;

        playedAnimation = false;
        animator.SetTrigger("Reset");
    }

    protected override void UpdateVisuals()
    {
        transform.position = GetClampedPosition(Utilities.Get2DMouseWorldPosition());
    }

    protected override void HandleSpin()
    {
        base.HandleSpin();

        Vector2 position = grabOriginalPosition + (Vector2.up * 5) + (Random.insideUnitCircle * priestParticleRange);
        Instantiate(waterParticles, position, Quaternion.identity).gameObject.SetActive(true);
        if(shakeWater != null) audioSource.PlayOneShot(shakeWater);
    }

    private Vector2 GetClampedPosition(Vector2 position)
    {
        Vector2 newPosition = position - initialGrabOffset;
        return Vector2.ClampMagnitude(newPosition - grabOriginalPosition, priestRotationRange) + grabOriginalPosition;
    }

    protected override void OnPointerDown()
    {
        grabOriginalPosition = transform.position;
        initialGrabOffset = (Vector3)Utilities.Get2DMouseWorldPosition() - transform.position;
        base.OnPointerDown();

        if(madPerson != null) madPerson.Play();

        timer = 0;
    }

    protected override void OnPointerUp()
    {
        transform.position = grabOriginalPosition;
        isWithinRange = false;
        animator.SetTrigger("Reset");

        if(madPerson != null) madPerson.Stop();

        base.OnPointerUp();
    }

    void OnDrawGizmos()
    {
        DrawXXL.DrawBasics2D.Circle(transform.position, findRange, Color.red);
    }

    void OnDrawGizmosSelected()
    {
        DrawXXL.DrawBasics2D.Circle(transform.position, foundRange, Color.violetRed);
    }
}