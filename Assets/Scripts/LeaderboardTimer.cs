using UnityEngine;

public class LeaderboardTimer : MonoBehaviour
{
    private float timer;

    void OnEnable()
    {
        GameManager.OnResetLevel += ResetLevel;
    }

    void OnDisable()
    {
        GameManager.OnResetLevel -= ResetLevel;
    }

    void Update()
    {
        timer += Time.unscaledDeltaTime;
    }

    private void ResetLevel()
    {
        timer = 0;
    }
}