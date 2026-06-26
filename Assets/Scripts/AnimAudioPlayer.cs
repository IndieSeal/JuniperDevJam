using UnityEngine;

public class AnimAudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    void OnEnable()
    {
        audioSource.Play();
    }
}