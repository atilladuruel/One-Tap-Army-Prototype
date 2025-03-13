using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;

    private void Start()
    {
        ShowMainMenu();
    }

    /// <summary>
    /// Loads the main game scene.
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene("Level1"); // Ensure the correct scene name
    }

    /// <summary>
    /// Shows the main menu panel and hides other UI elements.
    /// </summary>
    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    /// <summary>
    /// Shows the settings panel.
    /// </summary>
    public void ShowSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
