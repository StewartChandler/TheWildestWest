using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingPopUp : MonoBehaviour
{
    public GameObject mainMenu;       // Reference to the main menu panel
    public GameObject settingsPanel;  // Reference to the settings panel
    public GameObject firstSelectedMenu, firstSelectedSettings;

    public void ShowSettingsPanel()
    {
        mainMenu.SetActive(false);    // Hide the main menu
        settingsPanel.SetActive(true); // Show the settings panel

        // Clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        // Set a new selected object
        EventSystem.current.SetSelectedGameObject(firstSelectedSettings);
    }

    public void BackToMainMenu()
    {
        mainMenu.SetActive(true);     // Show the main menu
        settingsPanel.SetActive(false); // Hide the settings panel

    }
}
