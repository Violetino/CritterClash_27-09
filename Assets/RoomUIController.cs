using UnityEngine;
using Photon.Pun;
using TMPro;

public class RoomUIController : MonoBehaviour
{
    public GameObject startGameButton;
    public RoomManager roomManager;

    private void Start()
    {

    }

    // Character selection button
    public void OnCharacterButtonPressed(int characterIndex)
    {
        roomManager.SelectCharacter(characterIndex);
    }

    // Level selection button (for host)
    public void OnLevelButtonPressed(string levelIndex)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            roomManager.SelectLevel(levelIndex);
        }
    }

    // Start game button (for host)
    public void OnStartGameButtonPressed()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            roomManager.StartGame(); // Load selected level
        }
    }
}

