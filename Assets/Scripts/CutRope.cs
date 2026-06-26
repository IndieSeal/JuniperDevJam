using GogoGaga.OptimizedRopesAndCables;
using UnityEngine;

public class CutRope : MonoBehaviour
{
    [SerializeField] private Rope fakeRope;
    [SerializeField] private GameObject ropePhysicsReference;
    private GameObject ropePhysicsInstance;

    [SerializeField] private SpinMechanic spinner;
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip snip;
    [SerializeField] private AudioClip cut;

    void OnEnable()
    {
        GameManager.OnResetLevel += ResetLevel;
        spinner.OnSpinCompleted += PlaySnipSound;
    }

    void OnDisable()
    {
        GameManager.OnResetLevel -= ResetLevel;
        spinner.OnSpinCompleted -= PlaySnipSound;
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
            audioSource.PlayOneShot(cut);
        }
    }

    private void PlaySnipSound()
    {
        audioSource.PlayOneShot(snip);
    }
}