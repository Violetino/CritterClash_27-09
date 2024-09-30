using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        // Load the room selection/creation scene
        SceneManager.LoadScene("Lobby");
    }

    public void Options()
    {
        // Load options menu or settings
        // SceneManager.LoadScene("OptionsScene"); // Optional, if you have options
    }

    public void ExitGame()
    {
        // Exit the application
        Application.Quit();
    }
    public void StartLevel1()
    {
        // Load the room selection/creation scene
        SceneManager.LoadScene("Bushland_01");
    }

    public void StartLevel2()
    {
        // Load the room selection/creation scene
        SceneManager.LoadScene("OtherScene");
    }
}
