using System.Collections;
using UnityEngine;

public class SpinTrail : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprCenter;
    [SerializeField] private TrailRenderer trail;
    private Spinner currentSource;

    void Awake()
    {
        Spinner.OnSelected += UpdateTrail;
        Spinner.OnUnselected += RemoveTrail;
    }

    void Update()
    {
        if(currentSource == null) return;
        
        bool canBeDrawn = currentSource.GetProgress() < 1;
        if (canBeDrawn) trail.transform.position = Utilities.Get2DMouseWorldPosition();
    }

    private void UpdateTrail(Spinner newSource)
    {
        StopAllCoroutines();

        var previousSource = currentSource;
        currentSource = newSource;

        if(previousSource == currentSource) return;

        sprCenter.gameObject.SetActive(false);
        trail.emitting = false;

        if(currentSource != null)
        {
            StartCoroutine(HandleNewTrail());
            sprCenter.transform.position = Utilities.Get2DMouseWorldPosition();   
        }
    }

    public void RemoveTrail(Spinner newSource)
    {
        if(currentSource == newSource) UpdateTrail(null);
    }

    private IEnumerator HandleNewTrail()
    {
        yield return null;
        sprCenter.gameObject.SetActive(true);
        trail.emitting = true;
    }
}