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

    [Header("Audio")]
    [SerializeField] private AudioSource footstepL;
    [SerializeField] private AudioSource footstepR;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5;
    public bool IsAllowedToMove { get; set; } = true;

    [Header("Animation")]
    [SerializeField] private BouncyMation bouncyMation;
    private bool isMoving;

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
    private void SetInIndex(int index)
    {
        Stop(null);
        lastCheckpoint = index;
        OnReachCheckpoint?.Invoke(path.GetPathPoint(index));
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

        StopCoroutine(MoveSfx());
        StartCoroutine(MoveSfx());
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
                    isMoving = true;
                }
                else
                {
                    bouncyMation.Handle(Vector2.zero);
                    isMoving = false;
                }
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

            isMoving = false;
            yield return new WaitForSeconds(current.duration);

            current = path.GetPathPoint(++Index);
        }
    }

    private IEnumerator MoveSfx()
    {
        float footstepDelay = 0.4f;
        while (true)
        {
            yield return new WaitForSeconds(footstepDelay);
            if(isMoving) footstepL.Play();
            yield return new WaitForSeconds(footstepDelay);
            if(isMoving) footstepR.Play();
        }
    }

    public void Stop(Vampire vamp)
    {
        StopAllCoroutines();
    }
}