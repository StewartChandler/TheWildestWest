using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class InGameSettingsPopUp : MonoBehaviour
{
    // Find an object called SettingsPanel
    public GameObject settingsPanel;
    public GameObject firstSelectedSettings;
    private GameManager gameManager;
    Scene currentScene;
    private void Start()
    {
        currentScene = SceneManager.GetActiveScene();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    public void OnSettings()
    {
        currentScene = SceneManager.GetActiveScene();
        if (currentScene.name != "StartScene")
        {
            if (gameManager.isPaused == false)
            {
                GameObject playerManager = GameObject.Find("PlayerManager");

                PlayerInput[] playerInputs = playerManager.GetComponentsInChildren<PlayerInput>();

                foreach (PlayerInput playerInput in playerInputs)
                {
                    playerInput.DeactivateInput();
                }
                // set game to paused
                gameManager.isPaused = true;
                // pause game
                Time.timeScale = 0;
                settingsPanel.SetActive(true);
                // Clear selected object
                EventSystem.current.SetSelectedGameObject(null);
                // Set a new selected object
                EventSystem.current.SetSelectedGameObject(firstSelectedSettings);
            }
            else
            {
                // set game to unpaused
                gameManager.isPaused = false;
                // unpause game
                Time.timeScale = 1;
                settingsPanel.SetActive(false);
                GameObject playerManager = GameObject.Find("PlayerManager");

                PlayerInput[] playerInputs = playerManager.GetComponentsInChildren<PlayerInput>();
                for (int i = 0; i < playerInputs.Length; i++)
                {
                    if (gameManager.isPlayerAlive[i] == true)
                    {
                        playerInputs[i].ActivateInput();
                    }
                }
            }

        }
    }

    // Method to return to main menu
    public void OnMainMenu()
    {
        // set game to unpaused
        gameManager.isPaused = false;
        // unpause game
        Time.timeScale = 1;
        // Load the main menu scene
        SceneManager.LoadScene("StartScene");
        AudioManager.instance.Stop("GameMusic");
    }

    // Update is called once per frame
    void Update()
    {
        // if escape was pressed by keyboard, or start/select was pressed by controller
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Start"))
        {
            OnSettings();
        }
    }
}
