using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectOption : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject lockedPanel;
    [SerializeField] private Button button;

    [SerializeField] private float hoverIncreseTargetSize = 1.5f;
    [SerializeField] private float hoverIncreseSpeed = 1f;
    
    [SerializeField, Tooltip("To unlock"), HideIf("isUnlockedByDefault")] private string previousSceneName = "Hey";
    [SerializeField] private string sceneName = "Hey";
    [SerializeField] private bool lockedBecauseOfFixing;
    [SerializeField] private bool isUnlockedByDefault = false;
    private bool isUnlocked;

    private bool isHovered;

    void Awake()
    {
        isUnlocked = isUnlockedByDefault || PlayerPrefs.GetInt(previousSceneName, 0) == 1;
        if(lockedBecauseOfFixing) isUnlocked = false;

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

    void Update()
    {
        float targetScale = 1;
        if(isHovered) targetScale = hoverIncreseTargetSize;

        transform.localScale = Vector3.Lerp(transform.localScale, targetScale.ConvertToVector(), hoverIncreseSpeed * Time.deltaTime);
    }

    private void OnClickButton()
    {
        Debug.Log($"Changing scene to: {sceneName}");
        foreach(var button in FindObjectsByType<Button>()) button.interactable = false;
        StartCoroutine(TransitionManager.Instance.TransitionToNewScene(sceneName));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }
}