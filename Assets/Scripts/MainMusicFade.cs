using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MainMusicFade : MonoBehaviour
{
    private AudioSource mySource;
    
    void Awake()
    {
        mySource = GetComponent<AudioSource>();
        
        transform.parent = null;
        DontDestroyOnLoad(this);
    }

    void OnEnable()
    {
        SceneManager.sceneUnloaded += FadeMusicOut;
    }

    void OnDisable()
    {
        SceneManager.sceneUnloaded -= FadeMusicOut;
    }

    private void FadeMusicOut(Scene a)
    {
        StartCoroutine(LowerVolume(mySource));
    }

    private IEnumerator LowerVolume(AudioSource source, float speed = 0.4f)
    {
        while(source.volume > 0.05f)
        {
            source.volume -= speed * Time.deltaTime;
            yield return null;
        }

        source.volume = 0;
        Destroy(gameObject);
    }
}