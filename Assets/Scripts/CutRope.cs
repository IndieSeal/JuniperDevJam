using GogoGaga.OptimizedRopesAndCables;
using UnityEngine;

public class CutRope : MonoBehaviour
{
    [SerializeField] private Rope fakeRope;
    [SerializeField] private GameObject ropePhysicsReference;
    private GameObject ropePhysicsInstance;

    [SerializeField] private Spinner spinner;

    void OnEnable()
    {
        GameManager.OnResetLevel += ResetLevel;
    }

    void OnDisable()
    {
        GameManager.OnResetLevel -= ResetLevel;
    }

    private void ResetLevel()
    {
        fakeRope.gameObject.SetActive(true);
        if(ropePhysicsInstance != null) Destroy(ropePhysicsInstance);
    }

    void Update()
    {
        if(spinner.IsFinished && ropePhysicsInstance == null)
        {
            ropePhysicsInstance = Instantiate(ropePhysicsReference, ropePhysicsReference.transform.position, ropePhysicsReference.transform.rotation);
            ropePhysicsInstance.SetActive(true);
            fakeRope.gameObject.SetActive(false);
        }
    }
}