using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomCreationManager : MonoBehaviour
{
    public GameObject roomCreationPanel; // Reference to the Room Creation Panel
    public TMP_InputField roomNameInput; // Input field for the room name
    public TMP_Dropdown levelDropdown; // Dropdown for level selection
    private string selectedLevel;

    private void Start()
    {
        // Add level options to the dropdown
        levelDropdown.ClearOptions();
        levelDropdown.AddOptions(new List<string> { "Bushland_01", "OtherScene", "Bushland_01" }); // Example levels
        selectedLevel = levelDropdown.options[0].text; // Default to the first level
    }

    // Called when the player selects a level from the dropdown
    public void OnLevelSelected(int index)
    {
        selectedLevel = levelDropdown.options[index].text; // Store the selected level
    }

    // Called when the Create Room button is clicked
    public void CreateRoom()
    {
        string roomName = roomNameInput.text;

        if (!string.IsNullOrEmpty(roomName))
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 4; // Example max players

            // Store selected level in room properties
            Hashtable customRoomProperties = new Hashtable();
            customRoomProperties["selectedLevel"] = selectedLevel;
            roomOptions.CustomRoomProperties = customRoomProperties;

            // Create the room
            PhotonNetwork.CreateRoom(roomName, roomOptions);
            roomCreationPanel.SetActive(false); // Hide the room creation panel after creating the room
        }
        else
        {
            Debug.LogError("Room name is required.");
        }
    }
}
