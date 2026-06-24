using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectOption : MonoBehaviour
{
    [SerializeField] private GameObject lockedPanel;
    [SerializeField] private Button button;
    
    [SerializeField, Tooltip("To unlock"), HideIf("isUnlockedByDefault")] private string previousSceneName = "Hey";
    [SerializeField] private string sceneName = "Hey";
    [SerializeField] private bool isUnlockedByDefault = false;
    private bool isUnlocked;

    void Awake()
    {
        isUnlocked = isUnlockedByDefault || PlayerPrefs.GetInt(previousSceneName, 0) == 1;

        lockedPanel.SetActive(!isUnlocked);
        button.interactable = isUnlocked;
    }

    void OnEnable()
    {
        button.onClick.AddListener(OnClickButton);
    }

    void OnDisable()
    {
        button.onClick.RemoveListener(OnClickButton);
    }

    private void OnClickButton()
    {
        Debug.Log($"Changing scene to: {sceneName}");
        foreach(var button in FindObjectsByType<Button>()) button.interactable = false;
        StartCoroutine(TransitionManager.Instance.TransitionToNewScene(sceneName));
    }
}