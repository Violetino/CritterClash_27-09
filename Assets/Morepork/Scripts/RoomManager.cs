using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance;

    public GameObject[] characterPrefabs; // List of character prefabs for selection
    public string[] levels = { "Bushland_01", "OtherScene" };

    public Transform[] spawnPoints;

    public GameObject roomConnectScreen;
    public GameObject roomCamera;
    public GameObject nameUI;
    public GameObject connectingUI;

    [HideInInspector] public int kills = 0;
    [HideInInspector] public int deaths = 0;

    private string nickname = "unnamed";
    public string roomNameToJoin = "Test";

    private string selectedCharacter = "DefaultCharacter"; // Default character
    private string selectedLevel;

    private void Awake()
    {
        instance = this;
    }

    // Method to change the player's nickname
    public void ChangeNickname(string _name)
    {
        nickname = _name;
    }

    // Called when the join room button is pressed
    public void JoinRoomButtonPressed()
    {
        PhotonNetwork.JoinOrCreateRoom(roomNameToJoin, null, null);
        nameUI.SetActive(false);
        connectingUI.SetActive(true);
    }

    // Called when a player successfully joins a room
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        roomConnectScreen.SetActive(false);
        roomCamera.SetActive(false);
        SpawnPlayer();
    }

    // Method to spawn the player at a random spawn point
    public void SpawnPlayer()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Instantiate the character chosen by the player
        GameObject playerPrefab = GetSelectedCharacterPrefab();
        GameObject _player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity);

        // Set up the player for local control and network sync
        _player.GetComponent<PlayerSetup>().IsLocalPlayer();
        _player.GetComponent<HealthManager>().isLocalPlayer = true;
        _player.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, nickname);

        PhotonNetwork.LocalPlayer.NickName = nickname;

        // Notify GameManager about player respawn to re-enable controls
        Debug.Log("Player has respawned, calling OnPlayerRespawn.");
        GameManager.Instance.OnPlayerRespawn(_player); // Re-enable controls after respawn
    }

    // Store the selected character in Photon properties
    public void SelectCharacter(int characterIndex)
    {
        selectedCharacter = characterPrefabs[characterIndex].name;

        // Store this in player custom properties so it's networked
        Hashtable hash = new Hashtable();
        hash["selectedCharacter"] = selectedCharacter;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    // Get the selected character prefab
    private GameObject GetSelectedCharacterPrefab()
    {
        Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
        string characterName = (string)hash["selectedCharacter"];

        // Look for the character prefab in the array based on the player's selection
        foreach (GameObject character in characterPrefabs)
        {
            if (character.name == characterName)
            {
                return character;
            }
        }

        // Return the default character if no selection was found
        return characterPrefabs[0];
    }

    // Store the selected level in room custom properties
    public void SelectLevel(string levelName)
    {
        if (System.Array.Exists(levels, element => element == levelName))  // Ensure level is valid
        {
            selectedLevel = levelName;
            Debug.Log("Level Selected: " + selectedLevel);

            if (PhotonNetwork.IsMasterClient)  // Only Master Client (host) can set room properties
            {
                // Create a new Hashtable to store the room properties
                Hashtable roomProperties = new Hashtable();
                roomProperties["selectedLevel"] = selectedLevel;

                // Set the room properties with the selected level
                PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
            }
        }
        else
        {
            Debug.LogError("Invalid level selected.");
        }
    }

    // Load the selected level for all players
    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient) // Only the Master Client should load the level
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("selectedLevel"))
            {
                string levelName = (string)PhotonNetwork.CurrentRoom.CustomProperties["selectedLevel"];
                Debug.Log("Loading level: " + levelName);

                PhotonNetwork.LoadLevel(levelName); // Master Client loads the level for everyone
            }
            else
            {
                Debug.LogError("No level selected in room properties.");
            }
        }
        else
        {
            Debug.Log("Only the Master Client can start the game.");
        }
    }

    // Update player statistics
    public void SetHashes()
    {
        try
        {
            Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
            hash["kills"] = kills;
            hash["deaths"] = deaths;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
        catch
        {
            // Silently handle any exceptions related to setting properties
        }
    }
}
