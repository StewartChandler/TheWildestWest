using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InstructionsPopUp : MonoBehaviour
{
    public GameObject mainMenu;       // Reference to the main menu panel
    public GameObject instructionsPanel;  // Reference to the Instructions panel
    public GameObject firstSelectedMenu, firstSelectedInstructions;

    public void ShowInstructionsPanel()
    {
        mainMenu.SetActive(false);    // Hide the main menu
        instructionsPanel.SetActive(true); // Show the Instructions panel

        // Clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        // Set a new selected object
        EventSystem.current.SetSelectedGameObject(firstSelectedInstructions);
    }

    public void BackToMainMenu()
    {
        mainMenu.SetActive(true);     // Show the main menu
        instructionsPanel.SetActive(false); // Hide the Instructions panel

        // Clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        // Set a new selected object
        EventSystem.current.SetSelectedGameObject(firstSelectedMenu);

    }
}
