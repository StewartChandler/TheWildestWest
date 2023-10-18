using UnityEngine;
using UnityEngine.UI;

public class SettingPopUp : MonoBehaviour
{
    public GameObject mainMenu;       // Reference to the main menu panel
    public GameObject settingsPanel;  // Reference to the settings panel

    public void ShowSettingsPanel()
    {
        mainMenu.SetActive(false);    // Hide the main menu
        settingsPanel.SetActive(true); // Show the settings panel
    }

    public void BackToMainMenu()
    {
        mainMenu.SetActive(true);     // Show the main menu
        settingsPanel.SetActive(false); // Hide the settings panel
    }
}
