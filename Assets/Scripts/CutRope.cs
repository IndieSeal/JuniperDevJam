using GogoGaga.OptimizedRopesAndCables;
using UnityEngine;

public class CutRope : MonoBehaviour
{
    [SerializeField] private Rope fakeRope;
    [SerializeField] private GameObject ropePhysicsReference;
    private GameObject ropePhysicsInstance;

    [SerializeField] private Spinner spinner;

    void Awake()
    {
        GameManager.OnResetLevel += ResetLevel;
    }

    private void ResetLevel()
    {
        fakeRope.gameObject.SetActive(true);
        if(ropePhysicsInstance != null) Destroy(ropePhysicsInstance);
    }

    void Update()
    {
        bool hasCompleted = spinner.GetProgress() >= 1;
        if(hasCompleted && ropePhysicsInstance == null)
        {
            ropePhysicsInstance = Instantiate(ropePhysicsReference, ropePhysicsReference.transform.position, ropePhysicsReference.transform.rotation);
            ropePhysicsInstance.SetActive(true);
            fakeRope.gameObject.SetActive(false);

            spinner.PointerUp();
        }
    }
}