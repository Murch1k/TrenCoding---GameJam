using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject gameHUD;
    public GameObject playerObject; // Drag your Player into this slot

    void Start()
    {
        // Ensure the menu is the only thing active at the start
        ShowMenu();
    }

    public void PlayGame()
    {
        mainMenuPanel.SetActive(false); // Hide the menu
        gameHUD.SetActive(true);        // Show the HUD
        playerObject.SetActive(true);   // "Spawn" the player

        // Optional: If your game was paused
        Time.timeScale = 1f;
    }

    public void ShowMenu()
    {
        mainMenuPanel.SetActive(true);
        gameHUD.SetActive(false);
        playerObject.SetActive(false);

        // Optional: Freeze the game while in menu
        Time.timeScale = 0f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}