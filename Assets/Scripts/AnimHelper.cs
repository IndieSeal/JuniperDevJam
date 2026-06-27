using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class AnimHelper : MonoBehaviour
{
    public void BasicShake()
    {
        StopAllCoroutines();
        StartCoroutine(Shake(1, 1, 0.5f));
    }
    public void BigShake()
    {
        StopAllCoroutines();
        StartCoroutine(Shake(3, 2, 1.5f));
    }

    private IEnumerator Shake(float amplitude, float frequency, float time)
    {
        var camera = FindAnyObjectByType<CinemachineBasicMultiChannelPerlin>();
        camera.AmplitudeGain = amplitude;
        camera.FrequencyGain = frequency;

        yield return new WaitForSeconds(time);
        
        camera.AmplitudeGain = 0;
        camera.FrequencyGain = 0;
    }
}