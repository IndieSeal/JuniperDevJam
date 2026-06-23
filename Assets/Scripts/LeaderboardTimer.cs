using UnityEngine;

public class LeaderboardTimer : MonoBehaviour
{
    private float timer;

    void Awake()
    {
        GameManager.OnResetLevel += ResetLevel;
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