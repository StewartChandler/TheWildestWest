using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public PlayerInput[] players = new PlayerInput[4];
    public bool[] isPlayerAlive = { false, false, false, false };
    public int[] playerScores = { 0, 0, 0, 0 };
    public int playersReady = 0;
    public int numPlayers = 0;
    private static GameManager instance;
    // currentScene does not keep track of the actual current scene and should be refactored
    Scene currentScene;
    public Transform[] playerSpawns = new Transform[4];
    public GameObject playerPrefab;
    [SerializeField] private UnityEvent _onGameStart;

    public bool timerFinished = false;
    FadeInOut fade;
    private bool fadeing = true;


    private void Awake()
    {
        currentScene = SceneManager.GetActiveScene();
        fade = FindObjectOfType<FadeInOut>();

    }
    private void Update()
    {
        // refresh currentScene on scene change
        if (currentScene.name == "PlayerSelect")
        {
            if (numPlayers >= 2 && numPlayers == playersReady && timerFinished)
            {
                if (fadeing)
                {
                    ChangeSceneRoutine();
                }
                else
                {
                    for (int i = 0; i < playersReady; i++)
                    {
                        isPlayerAlive[i] = true;
                    }
                    Debug.Log("Starting Game");
                    _onGameStart.Invoke();
                }
            }
        }
        if (SceneManager.GetActiveScene().name == "NewMap")
        {
            if (isPlayerAlive.Count(x => x) <= 1)
            {
                // add a point to the player(s) alive at end of round
                bool someoneWon = false;
                for (int i = 0; i < playersReady; i++)
                {
                    if (isPlayerAlive[i])
                    {
                        playerScores[i]++;
                        // see if a player has won
                        if (playerScores[i] >= 3)
                        {
                            someoneWon = true;
                        }
                    }
                    else
                    {
                        isPlayerAlive[i] = true;
                    }
                }
                // manage the hats
                ManageHats();

                // if someone has won, go to the end screen, else reset the game arena
                GameObject playerManager = GameObject.Find("PlayerManager");
                PlayerInput[] playerInputs = playerManager.GetComponentsInChildren<PlayerInput>();
                if (!someoneWon)
                {
                    // activate playerinput and reset the scene
                    foreach (PlayerInput playerInput in playerInputs)
                    {
                        // move each player to their respective spawn point
                        CharacterController characterController = playerInput.GetComponent<CharacterController>();
                        characterController.enabled = false;
                        playerInput.transform.position = playerSpawns[playerInput.playerIndex].position;
                        characterController.enabled = true;
                        playerInput.ActivateInput();
                    }
                    SwitchScene();
                }
                else
                {
                    foreach (PlayerInput playerInput in playerInputs)
                    {
                        playerInput.DeactivateInput();
                    }
                    SceneManager.LoadScene("EndScene");
                }
            }
        }
    }

    public void ChangeSceneRoutine()
    {
        StartCoroutine(ChangeScene());
    }

    public IEnumerator ChangeScene()
    {
        fade.FadeIn();
        yield return new WaitForSeconds(fade.TimeToFade);
        fadeing = false;

    }

    public void SwitchScene()
    {
        SceneManager.LoadScene("NewMap");
    }

    private void ManageHats()
    {
        GameObject playerManager = GameObject.Find("PlayerManager");
        // deal with all the hats safely by getting them all off first
        PlayerController[] playerControllers = playerManager.GetComponentsInChildren<PlayerController>();
        foreach (PlayerController playerController in playerControllers)
        {
            HatStack hatStack = playerController.GetComponentInChildren<HatStack>();
            if (hatStack != null)
            {
                hatStack.popAllHats();
                Debug.Log(hatStack.getNumHats());
            }
        }

        // destroy all the hats
        GameObject[] hats = GameObject.FindGameObjectsWithTag("Hat");
        foreach (GameObject hat in hats)
        {
            Destroy(hat);
        }

        // regenerate new hats for each player
        foreach (PlayerController playerController in playerControllers)
        {
            HatStack hatStack = playerController.GetComponentInChildren<HatStack>();
            if (hatStack != null)
            {
                hatStack.resetHats();
            }
        }
    }

    public void StartGame()
    {
        // Add logic to start the game here
        // Load your game scene or perform any necessary initialization.
        currentScene = SceneManager.GetActiveScene();


        if (numPlayers == 0 || numPlayers == 1)
        {

            // Instantiate player input for Player 2 (KeyboardLeft)
            var player2 = Instantiate(playerPrefab, playerSpawns[2].position, playerSpawns[2].rotation, gameObject.transform);
            player2.GetComponent<PlayerInput>().SwitchCurrentControlScheme("KeyboardLeft", Keyboard.current);
            players[1] = player2.GetComponent<PlayerInput>();
            player2.name = "Player2";
        }
        else
        {
            // iterate through the players and set the spawn points
            for (int i = 0; i < numPlayers; i++)
            {
                Debug.Log("CHANGING POISITON");
                CharacterController characterController = players[i].GetComponent<CharacterController>();
                characterController.enabled = false;
                players[i].transform.position = playerSpawns[i].position;
                characterController.enabled = true;
            }
        }


    }
}
