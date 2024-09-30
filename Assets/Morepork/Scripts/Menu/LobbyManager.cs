using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public GameObject roomListItemPrefab; // Prefab for each room item in the list
    public Transform roomListContainer; // Where the room list items will appear

    // Called when the player joins the lobby
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined the lobby.");
    }

    // Called when the room list is updated (e.g., when rooms are created/destroyed)
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // Clear the old room list
        foreach (Transform child in roomListContainer)
        {
            Destroy(child.gameObject);
        }

        // Populate the room list with available rooms
        foreach (RoomInfo room in roomList)
        {
            GameObject roomItem = Instantiate(roomListItemPrefab, roomListContainer);
            roomItem.GetComponentInChildren<TMP_Text>().text = room.Name; // Display room name

            // Add OnClick event to join the room
            roomItem.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                JoinRoom(room.Name);
            });
        }
    }

    // Method to join a room
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    // Called when the player successfully joins a room
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined the room successfully.");
        // Load character selection scene or panel here (if in the same scene)
        // SceneManager.LoadScene("CharacterSelectScene"); // Example
    }
}


