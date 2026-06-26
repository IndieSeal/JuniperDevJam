using System.Collections;
using DrawXXL;
using UnityEngine;

public class ChaseVampire : MonoBehaviour
{
    [SerializeField] private GameObject explodeGuardsParticles;

    [SerializeField] private float velocity = 5;
    [SerializeField] private float findRange = 5;
    [SerializeField] private BouncyMation bouncyMation;
    private Vector2? foundPosition;
    private bool reachedFoundPosition;
    private int? foundIndex;
    private Path path;
    private bool hasStartedCoroutine;

    private Vector2 firstPosition;

    void Awake()
    {
        firstPosition = transform.position;
    }

    void OnEnable()
    {
        Instantiate(explodeGuardsParticles, transform.position, Quaternion.identity);
        GameManager.OnResetLevel += ResetLevel;
    }

    void OnDestroy()
    {
        GameManager.OnResetLevel -= ResetLevel;
    }

    void Update()
    {
        if(foundPosition.HasValue) GoTowardsVampire();
        else FindVampire();
    }

    private void GoTowardsVampire()
    {
        if(!reachedFoundPosition)
        {
            transform.position = Vector2.MoveTowards(transform.position, foundPosition.Value, velocity * Time.deltaTime);
            bouncyMation.Handle((foundPosition.Value - (Vector2)transform.position).normalized);
            if(Vector2.Distance(transform.position, foundPosition.Value) < 0.05f) reachedFoundPosition = true;
        }
        else if(!hasStartedCoroutine)
        {
            StartCoroutine(MoveCoroutine());
            hasStartedCoroutine = true;
        }
    }

    private IEnumerator MoveCoroutine()
    {
        PathPoint current = path.GetPathPoint(foundIndex.Value);

        bouncyMation.Handle(Vector2.zero);
        bool IsAllowedToMove = true;
        while(current != null)
        {            
            Vector3 targetPosition = current.transform.position;
            while (transform.position != targetPosition)
            {
                if (IsAllowedToMove)
                {
                    transform.position = Vector2.MoveTowards(transform.position, targetPosition, velocity * Time.deltaTime);
                    bouncyMation.Handle((targetPosition - transform.position).normalized);
                }
                else bouncyMation.Handle(Vector2.zero);
                yield return null;
            }

            bouncyMation.Handle(Vector2.zero);
            current.CallEvents();

            yield return new WaitForSeconds(current.duration);

            foundIndex++;
            current = path.GetPathPoint(foundIndex.Value);
        }
    }

    private void FindVampire()
    {
        var collisions = Physics2D.OverlapCircleAll(transform.position, findRange);
        foreach (Collider2D collision in collisions)
        {
            if(!collision.TryGetComponent(out Vampire vampire)) continue;

            PathFollower pathFollower = vampire.GetComponent<PathFollower>();
            foundIndex = pathFollower.Index;

            path = pathFollower.Path;

            foundPosition = vampire.transform.position;
        }
    }

    private void ResetLevel()
    {
        StopAllCoroutines();

        transform.position = firstPosition;

        foundIndex = null;
        foundPosition = null;
        reachedFoundPosition = false;

        path = null;
        hasStartedCoroutine = false;

        gameObject.SetActive(true);
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
        if(go.CompareTag("KillBox") && go.TryGetComponent(out SpinnerCollider spinnerCollider) && spinnerCollider.canKillGuards)
        {
            Instantiate(explodeGuardsParticles, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }
    }

    void OnDrawGizmosSelected()
    {
        DrawBasics2D.Circle(transform.position, findRange);
    }
}