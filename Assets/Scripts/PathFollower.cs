using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public static event Action<PathPoint> OnReachCheckpoint;
    
    [Header("Traversal")]
    [SerializeField] private Path path;
    private int lastCheckpoint = -1;
    public int Index { get; private set; } = 0;

    public Path Path => path;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5;
    public bool IsAllowedToMove { get; set; } = true;

    [Header("Animation")]
    [SerializeField] private BouncyMation bouncyMation;

    void OnEnable()
    {
        GameManager.OnResetLevel += ResetPath;
        Vampire.OnDeath += Stop;
    }

    void OnDisable()
    {
        GameManager.OnResetLevel -= ResetPath;
        Vampire.OnDeath -= Stop;
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
        Index = lastCheckpoint == -1 ? 0 : lastCheckpoint;
        
        PathPoint current = path.GetPathPoint(Index);

        transform.position = current.transform.position;
        bouncyMation.Handle(Vector2.zero);

        while(current != null)
        {            
            Vector3 targetPosition = current.transform.position;
            while (transform.position != targetPosition)
            {
                if (IsAllowedToMove)
                {
                    transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
                    bouncyMation.Handle((targetPosition - transform.position).normalized);
                }
                else bouncyMation.Handle(Vector2.zero);
                yield return null;
            }

            //REACHED POINT
            if(Index > lastCheckpoint && current.checkpoint)
            {
                lastCheckpoint = Index;
                OnReachCheckpoint?.Invoke(current);
            }
            bouncyMation.Handle(Vector2.zero);
            current.CallEvents();

            yield return new WaitForSeconds(current.duration);

            current = path.GetPathPoint(++Index);
        }
    }

    public void Stop(Vampire vamp)
    {
        StopAllCoroutines();
    }
}