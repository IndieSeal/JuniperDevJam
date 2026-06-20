using System.Collections.Generic;
using System.Linq;
using LootLocker.Requests;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardController : MonoBehaviour
{
    public static LeaderboardController Instance;
    
    [Header("Leaderboard SDK")]
    [SerializeField] private string leaderboardId = "35034";
    private string playerId;
    private string playerUlid;
    private int latestScore;
    private string currentPlayerName;

    [Header("Player Information UI")]
    [SerializeField] private TMP_Text playerInfo_Text;

    [Header("Submit Score UI")]
    [SerializeField] private GameObject submitScore_Panel;
    [Space]
    [SerializeField] private TMP_InputField playerName_Field;
    [SerializeField] private TMP_Text playerName_Text;
    [Space]
    [SerializeField] private Button submit_Btn;

    private bool DoesPlayerHaveAnId => !string.IsNullOrEmpty(playerId);
    private bool DoesPlayerHaveAnUlid => !string.IsNullOrEmpty(playerUlid);

    #region Setup

    void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(this);

        playerName_Field.onValueChanged.AddListener(delegate { IsValidUsername(); } );
        submit_Btn.onClick.AddListener(RegisterNameAndSubmitScore);

        submitScore_Panel.SetActive(false);
        submit_Btn.interactable = false;
    }

    void Start()
    {
        StartNewGuestSession();
    }

    #endregion
    #region Public functions

    [Button]
    public void TrySubmitScore(int score)
    {
        latestScore = score;

        if (!DoesPlayerHaveAnId)
        {
            Debug.LogWarning("Cannot submit score yet. LootLocker session is not ready.");
            return;
        }

        IsUserRegistered(isRegistered => {
            if (isRegistered) SubmitScoreWithCurrentPlayer();
            else OpenSubmitScorePanel();
        });
    }

    [Button]
    public void SignOut()
    {
        if (!DoesPlayerHaveAnUlid)
        {
            Debug.LogWarning("No LootLocker user is currently signed in.");
            return;
        }

        bool shouldClearLocalState = true;

        LootLockerSDKManager.EndSession(response => {
            if (response.success)
            {
                playerId = null;
                playerUlid = null;
                currentPlayerName = null;

                submitScore_Panel.SetActive(false);
                submit_Btn.interactable = false;
                playerName_Field.text = "";

                StartNewGuestSession();
            }
            else Debug.LogWarning("Failed to sign out");
        }, shouldClearLocalState, playerUlid);
    }

    #endregion

    private void StartNewGuestSession()
    {
        LootLockerSDKManager.StartGuestSession(response => {
            if (!response.success)
            {
                Debug.LogWarning("Failed to start new guest session");
                return;
            }

            playerId = response.player_id.ToString();
            playerUlid = response.player_ulid;

            UpdateUserInformationUI();
        });
    }

    private void RegisterNameAndSubmitScore()
    {
        string playerName = playerName_Field.text.Trim();

        if (!IsValidUsernameLocal(playerName) || !DoesPlayerHaveAnId) return;

        submit_Btn.interactable = false;
        LootLockerSDKManager.SetPlayerName(playerName, nameResponse => {
            if (!nameResponse.success)
            {
                playerName_Text.color = Color.red;
                playerName_Text.text = "Player Name is already in use!";
                submit_Btn.interactable = true;
                return;
            }

            currentPlayerName = playerName;
            submitScore_Panel.SetActive(false);

            SubmitScoreWithCurrentPlayer();
        });
    }

    private void OpenSubmitScorePanel()
    {
        submitScore_Panel.SetActive(true);

        playerName_Field.text = "";
        playerName_Field.interactable = true;

        playerName_Text.color = Color.white;
        playerName_Text.text = "Choose a player name";

        submit_Btn.interactable = false;
    }

    private void SubmitScoreWithCurrentPlayer()
    {
        LootLockerSDKManager.SubmitScore("", latestScore, leaderboardId, scoreResponse => {
            if (scoreResponse.success)
            {
                playerName_Text.color = Color.green;
                playerName_Text.text = "Score submitted!";
                submit_Btn.interactable = false;

                UpdateUserInformationUI();
            }
            else
            {
                playerName_Text.color = Color.red;
                playerName_Text.text = "Failed to submit score!";
                submit_Btn.interactable = true;
            }
        });
    }

    private void UpdateUserInformationUI()
    {
        RefreshEntries();
        
        if (!DoesPlayerHaveAnId)
        {
            playerInfo_Text.text = "Not signed in";
            return;
        }

        IsUserRegistered(isRegistered => {
            if (!isRegistered)
            {
                playerInfo_Text.text = "";
                return;
            }

            LootLockerSDKManager.GetMemberRank(leaderboardId, playerId, response => {
                playerInfo_Text.text = $"User: {currentPlayerName}";
                if (!response.success) return;

                string rankText = response.rank > 0 ? $" #{response.rank}" : "";
                string highText = response.score >= 0 ? $", {response.score}Hs" : "";

                playerInfo_Text.text += $"{rankText}{highText}";
            });
        });
    }

    private void IsUserRegistered(System.Action<bool> onComplete)
    {
        LootLockerSDKManager.GetPlayerName(response =>
        {
            if (!response.success)
            {
                Debug.LogWarning("Failed to get player name");
                currentPlayerName = null;
                onComplete?.Invoke(false);
                return;
            }

            currentPlayerName = response.name;

            bool isRegistered = !string.IsNullOrEmpty(currentPlayerName);
            onComplete?.Invoke(isRegistered);
        });
    }

    #region User Creation
    
    private bool IsValidUsername()
    {
        string playerName = playerName_Field.text.Trim();
        submit_Btn.interactable = false;

        if (!IsValidUsernameLocal(playerName)) return false;

        playerName_Text.color = Color.white;
        playerName_Text.text = "Checking player name...";

        DoesPlayerNameExist(playerName, exists =>
        {
            if (exists)
            {
                playerName_Text.color = Color.red;
                playerName_Text.text = "Player Name is already in use!";
                submit_Btn.interactable = false;
            }
            else
            {
                playerName_Text.color = Color.green;
                playerName_Text.text = "Player Name can be used!";
                submit_Btn.interactable = true;
            }
        });

        return true;
    }

    private bool IsValidUsernameLocal(string text)
    {
        playerName_Text.color = Color.red;

        if (string.IsNullOrEmpty(text))
        {
            playerName_Text.text = "Player Name Field is Empty!";
            return false;
        }

        if (text.Length < 3)
        {
            playerName_Text.text = "Player Name Field must be of 3 characters or more!";
            return false;
        }

        return true;
    }

    private void DoesPlayerNameExist(string name, System.Action<bool> onComplete)
    {
        string[] playerNames = { name };

        LootLockerSDKManager.LookupPlayerNamesByPlayerNames(playerNames, response =>
        {
            if (!response.success)
            {
                Debug.LogError("Couldn't fetch player names");
                onComplete?.Invoke(true);
                return;
            }

            bool exists = response.players != null && response.players.Any(player => player != null && string.Equals(player.name, name, System.StringComparison.OrdinalIgnoreCase));
            onComplete?.Invoke(exists);
        });
    }

    #endregion
    #region Leaderboards

    public const int MAX_SCORES = 10;

    [Header("Leaderboard User List")]
    [SerializeField] private LeaderboardEntry entriesPrefab;
    [SerializeField] private Transform entriesParent;
    private List<LeaderboardEntry> entriesInstances = new List<LeaderboardEntry>();

    private void RefreshEntries()
    {
        CreateEntries();

        LootLockerSDKManager.GetScoreList(leaderboardId, MAX_SCORES, response => {
            if (!response.success)
            {
                Debug.LogWarning("Couldn't load leaderboard/get scores");
                return;
            }

            // Leaderboard is empty :[
            if (response.items == null || response.items.Length == 0) return;

            for (int i = 0; i < entriesInstances.Count; i++)
            {
                entriesInstances[i].userAndScore.text = "None";
                entriesInstances[i].rank.text = "";
            }

            for (int i = 0; i < response.items.Length; i++)
            {
                var score = response.items[i];
                if (score == null || entriesInstances[i] == null) continue;

                string displayName = "Unknown";
                if (score.player != null && !string.IsNullOrEmpty(score.player.name)) displayName = score.player.name;
                else if (!string.IsNullOrEmpty(score.member_id)) displayName = score.member_id;

                entriesInstances[i].userAndScore.text = $"{displayName}\n{score.score}";
                entriesInstances[i].rank.text = $"#{score.rank}";
            }
        });
    }

    private void CreateEntries()
    {
        if(entriesInstances.Count != 0) return;

        for(int i = 0; i < MAX_SCORES; i++)
        {
            var instance = Instantiate(entriesPrefab, entriesParent);
            instance.userAndScore.text = "Loading";
            instance.rank.text = "";
            
            entriesInstances.Add(instance);
        }
    }

    #endregion
}