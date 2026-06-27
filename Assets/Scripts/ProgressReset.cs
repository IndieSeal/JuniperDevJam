using UnityEngine;
using UnityEngine.UI;

public class ProgressReset : MonoBehaviour
{
    [SerializeField] private Button button;

    void OnEnable()
    {
        button.onClick.AddListener(OnPress);
    }

    void OnDisable()
    {
        button.onClick.RemoveListener(OnPress);
    }

    private void OnPress()
    {
        foreach(var option in FindObjectsByType<LevelSelectOption>())
        {
            PlayerPrefs.SetInt(option.SceneName, 0);
        }

        foreach(var button in FindObjectsByType<Button>()) button.interactable = false;
        StartCoroutine(TransitionManager.Instance.TransitionToNewScene("LevelSelect"));
    }
}