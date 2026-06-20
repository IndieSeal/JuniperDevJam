using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardTest : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button submit;

    [SerializeField] private Button signOut;

    void Start()
    {
        submit.onClick.AddListener(SubmitScore);
        signOut.onClick.AddListener(LeaderboardController.Instance.SignOut);
    }

    private void SubmitScore()
    {
        LeaderboardController.Instance.TrySubmitScore(int.Parse(inputField.text));
    }
}