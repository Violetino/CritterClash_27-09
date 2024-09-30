using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Collections;
using Morepork.FinalCharacterController;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance; // Singleton instance
    public Leaderboard leaderboard;

    // Public game settings
    public float roundDuration = 120f; // 2 minutes
    public float leaderboardDuration = 30f; // 30 seconds

    private bool roundActive = false;
    private double roundStartTime; // Network time when the round starts
    private float currentRoundTime;

    [SerializeField] private TextMeshProUGUI timerText; // UI Text element to display the round timer

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            OnFirstPlayerSpawn();
        }
    }

    public void OnFirstPlayerSpawn()
    {
        if (!roundActive && PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.Time > 0)
            {
                roundStartTime = PhotonNetwork.Time;
                photonView.RPC("RPC_StartRound", RpcTarget.AllBuffered, roundStartTime);
            }
            else
            {
                Debug.LogError("PhotonNetwork.Time is not initialized properly.");
            }
        }
    }

    [PunRPC]
    public void RPC_StartRound(double startTime)
    {
        roundStartTime = startTime;
        currentRoundTime = roundDuration;
        roundActive = true;
        StartCoroutine(RoundCountdown());
    }

    private IEnumerator RoundCountdown()
    {
        while (roundActive)
        {
            double elapsedTime = PhotonNetwork.Time - roundStartTime;
            currentRoundTime = Mathf.Max(0, roundDuration - (float)elapsedTime);

            if (timerText != null)
            {
                UpdateTimerText(currentRoundTime);
            }

            if (currentRoundTime <= 0)
            {
                EndRound();
                yield break;
            }

            yield return null;
        }
    }

    private void EndRound()
    {
        roundActive = false;

        // Disable controls for all players at the end of the round
        DisableAllPlayerControls();

        // Show leaderboard and reset round
        StartCoroutine(ShowLeaderboard());
    }

    private IEnumerator ShowLeaderboard()
    {
        ShowLeaderboardUI();
        yield return new WaitForSeconds(leaderboardDuration);
        HideLeaderboardUI();
        ResetRound();
    }

    private void DisableAllPlayerControls()
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();

        foreach (var player in players)
        {
            player.DisableControls(); // This method disables controls for each player
        }
    }

    private void ShowLeaderboardUI()
    {
        Debug.Log("Showing leaderboard...");
        leaderboard.ShowLeaderboard();
    }

    private void HideLeaderboardUI()
    {
        Debug.Log("Hiding leaderboard...");
        leaderboard.HideLeaderboard();
    }

    private void ResetRound()
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();

        foreach (var player in players)
        {
            PhotonView photonView = player.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                PhotonNetwork.Destroy(photonView.gameObject);
            }
        }

        RoomManager.instance.SpawnPlayer();

        if (PhotonNetwork.IsMasterClient)
        {
            OnFirstPlayerSpawn();
        }
    }

    private void UpdateTimerText(float timeRemaining)
    {
        if (timerText == null)
        {
            Debug.LogError("TimerText is not assigned in GameManager.");
            return;
        }

        timeRemaining = Mathf.Max(0, timeRemaining);
        int minutes = Mathf.FloorToInt(timeRemaining / 60F);
        int seconds = Mathf.FloorToInt(timeRemaining % 60F);
        string timeFormatted = string.Format("{0:0}:{1:00}", minutes, seconds);

        timerText.text = timeFormatted;
    }

    public bool IsRoundActive()
    {
        return roundActive;
    }

    // Call this method when a player dies
    public void OnPlayerDeath(GameObject player)
    {
        PlayerSetup playerSetup = player.GetComponent<PlayerSetup>();
        if (playerSetup != null)
        {
            playerSetup.OnPlayerDeath(); // Disable controls for the player who died
        }
    }

    // Call this method when a player respawns
    public void OnPlayerRespawn(GameObject player)
    {
        PlayerSetup playerSetup = player.GetComponent<PlayerSetup>();
        if (playerSetup != null)
        {
            playerSetup.OnPlayerRespawn(); // Re-enable controls for the respawning player
        }
    }
}

