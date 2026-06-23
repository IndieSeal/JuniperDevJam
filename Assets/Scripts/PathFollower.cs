using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    [Header("Traversal")]
    [SerializeField] private Path path;
    private int index = 0;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5;

    [Header("Animation")]
    [SerializeField] private BouncyMation bouncyMation;

    void Awake()
    {
        GameManager.OnResetLevel += ResetPath;
        Vampire.OnDeath += Stop;
    }

    void Start()
    {
        ResetPath();
    }

    [Button]
    private void ResetPath()
    {
        if(path == null)
        {
            Debug.LogWarning("No path to follow, be sure to set it");
            return;
        }
        if(!Application.isPlaying) return;
        
        StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        gameObject.SetActive(true);
        index = 0;
        
        PathPoint current = path.GetPathPoint(index);

        transform.position = current.transform.position;
        bouncyMation.Handle(Vector2.zero);

        while(current != null)
        {
            Vector3 targetPosition = current.transform.position;
            while (transform.position != targetPosition)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
                bouncyMation.Handle((targetPosition - transform.position).normalized);
                yield return null;
            }

            bouncyMation.Handle(Vector2.zero);

            yield return new WaitForSeconds(current.duration);

            current = path.GetPathPoint(++index);
        }
    }

    public void Stop(Vampire vamp)
    {
        StopAllCoroutines();
    }
}