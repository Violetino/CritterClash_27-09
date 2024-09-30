using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;

public class CharacterSelectManager : MonoBehaviour
{
    public TMP_InputField nicknameInput;
    private string selectedCharacter;

    public void SelectCharacter(int characterIndex)
    {
        selectedCharacter = RoomManager.instance.characterPrefabs[characterIndex].name;
        RoomManager.instance.SelectCharacter(characterIndex);  // Store selected character in RoomManager
    }

    public void SetNickname()
    {
        string nickname = nicknameInput.text;
        if (!string.IsNullOrEmpty(nickname))
        {
            RoomManager.instance.ChangeNickname(nickname);
        }
    }

    public void StartGame()
    {
        // Notify RoomManager to start the game and load the level
        RoomManager.instance.StartGame();
    }
}

