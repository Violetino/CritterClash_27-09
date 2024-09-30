using UnityEngine;
using System.Linq;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class Leaderboard : MonoBehaviour
{
    public GameObject playersHolder; // Reference to the holder object that contains player leaderboard UI

    [Header("Options")]
    public float refreshRate = 1f; // Refresh rate for updating the leaderboard

    [Header("UI")]
    public GameObject[] slots; // Array of UI slots for displaying player information
    public TextMeshProUGUI[] scoreTexts; // Text components for displaying player scores
    public TextMeshProUGUI[] nameTexts;  // Text components for displaying player names
    public TextMeshProUGUI[] kdTexts; // Text components for displaying kill/death ratios

    private bool isLeaderboardVisible = false; // Track if leaderboard is programmatically visible (e.g., end of round)
    private bool isTabPressed = false; // Track if the leaderboard is being shown via the Tab key

    private void Start()
    {
        // Start refreshing the leaderboard UI based on the refresh rate
        InvokeRepeating(nameof(Refresh), 1f, refreshRate);
    }

    // Method to update the leaderboard UI
    public void Refresh()
    {
        foreach (var slot in slots)
        {
            slot.SetActive(false); // Deactivate all slots initially
        }

        var sortedPlayerList = PhotonNetwork.PlayerList
            .OrderByDescending(player => player.GetScore()) // Sort players by score
            .ThenBy(player => player.NickName) // Sort by name if scores are equal
            .ToList();

        int i = 0;
        foreach (var player in sortedPlayerList)
        {
            if (i >= slots.Length) break; // Stop if more players than available slots

            slots[i].SetActive(true); // Activate the slot for this player

            if (string.IsNullOrEmpty(player.NickName)) player.NickName = "unnamed";

            nameTexts[i].text = player.NickName; // Update player name
            scoreTexts[i].text = player.GetScore().ToString(); // Update player score

            if (player.CustomProperties["kills"] != null)
            {
                kdTexts[i].text = player.CustomProperties["kills"] + "/" + player.CustomProperties["deaths"]; // Update K/D
            }
            else
            {
                kdTexts[i].text = "0/0"; // Default K/D if not available
            }

            i++;
        }
    }

    // Update method to show/hide the leaderboard with the Tab key
    public void Update()
    {
        // If Tab is pressed, show the leaderboard and set the isTabPressed flag to true
        //if (Input.GetKeyDown(KeyCode.Tab))
        //{
        //    isTabPressed = true;
        //    ShowLeaderboard();
        //}

        // If Tab is released, hide the leaderboard unless it's programmatically set to stay visible
        //if (Input.GetKeyUp(KeyCode.Tab))
        //{
        //    isTabPressed = false;
        //    if (!isLeaderboardVisible)
        //    {
        //       HideLeaderboard();
        //    }
        //}
    }

    // Display leaderboard manually (e.g., end of round)
    public void ShowLeaderboard()
    {
        playersHolder.SetActive(true);
        isLeaderboardVisible = true; // Track that it's visible programmatically
    }

    // Hide leaderboard manually (e.g., after showing for a few seconds)
    public void HideLeaderboard()
    {
        // Only hide the leaderboard if it's not being shown via the Tab key
        if (!isTabPressed)
        {
            playersHolder.SetActive(false);
            isLeaderboardVisible = false; // Track that it's hidden programmatically
        }
    }

    // Reset leaderboard state for when it's triggered via game logic
    public void DisplayLeaderboard()
    {
        ShowLeaderboard();
    }

    public void HideLeaderboardViaLogic()
    {
        HideLeaderboard();
    }
}



