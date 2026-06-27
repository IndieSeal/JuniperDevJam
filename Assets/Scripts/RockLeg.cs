using UnityEngine;

public class RockLeg : MonoBehaviour
{
    [SerializeField] private GameObject breaking;

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
        breaking.SetActive(false);
    }

    void Update()
    {
        if(spinner.IsFinished && !breaking.activeSelf)
        {
            breaking.SetActive(true);
            audioSource.PlayOneShot(cut);
        }
    }

    private void PlaySnipSound()
    {
        audioSource.PlayOneShot(snip);
    }
}