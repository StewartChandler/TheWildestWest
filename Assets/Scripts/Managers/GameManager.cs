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
    public int[] playerHats = { 0, 0, 0, 0 };
    public int[] playerRoundHats = { 0, 0, 0, 0 };
    public Color[] playerColors = { new Color(220f / 255f, 78f / 255f, 50f / 255f),
                                    new Color(86f / 255f, 110f / 255f, 61f / 255f),
                                    new Color(114f / 255f, 16f / 255f, 117f / 255f),
                                    new Color(256f / 255f, 256f / 255f, 256f / 255f)};
    public Material[] playerMaterials = new Material[4];
    public int playersReady = 0;
    public int numPlayers = 0;
    public int currRound = 1;
    private static GameManager instance;
    // currentScene does not keep track of the actual current scene and should be refactored
    Scene currentScene;
    public Transform[] playerSpawns = new Transform[4];
    public GameObject playerPrefab;
    [SerializeField] private UnityEvent _onGameStart;
    public bool testStart = false;
    public bool firstGame = true;
    public bool timerFinished = false;
    FadeInOut fade;
    private bool fadeing = true;
    public bool endScreen = false;

    public EndRoundScreen endRoundScreen;
    public bool firstPass = true;

    public bool ending = false;
    public int currWinner = 0;
    bool someoneWon = false;
    public bool isPaused = false;

    private bool trainCalled = false;
    public bool timeRanOut = false;

    public int maxScore = 3;


    private void Awake()
    {
        currentScene = SceneManager.GetActiveScene();
        fade = FindObjectOfType<FadeInOut>();
        playerMaterials[0] = Resources.Load<Material>("Player/PlayerRed");
        playerMaterials[1] = Resources.Load<Material>("Player/PlayerGreen");
        playerMaterials[2] = Resources.Load<Material>("Player/PlayerPurple");
        playerMaterials[3] = Resources.Load<Material>("Player/PlayerWhite");
        Debug.Log("HERE");
        Debug.Log(Resources.Load<Material>("Player/PlayerGreen"));
    }
    private void Update()
    {
        // destroy game manager if in Start Scene
        if (currentScene.name == "StartScene")
        {
            Destroy(gameObject);
        }
        // refresh currentScene on scene change
        if (currentScene.name == "PlayerSelect" || currentScene.name == "PlayerSelectMap")
        {
            if (testStart)
            {
                // Instantiate player input for Player 2 (KeyboardLeft)
                var player2 = Instantiate(playerPrefab, playerSpawns[2].position, playerSpawns[2].rotation, gameObject.transform);
                player2.GetComponent<PlayerInput>().SwitchCurrentControlScheme("KeyboardLeft", Keyboard.current);
                players[1] = player2.GetComponent<PlayerInput>();
                player2.name = "Player2";
                // numPlayers++;
                // playersReady++;
                // timerFinished = true;
                testStart = false;

            }
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
            if (!trainCalled)
            {
                GameObject train = GameObject.FindGameObjectWithTag("Train");
                TrainLogic trainLogic = train.GetComponentInChildren<TrainLogic>();
                trainLogic.InvokeRepeating("TrainEvent", 5.0f, 15.0f);
                trainCalled = true;
            }

            if (isPlayerAlive.Count(x => x) <= 1)
            {

                // add a point to the player(s) alive at end of round
                if (firstPass)
                {
                    firstPass = false;
                    for (int i = 0; i < playersReady; i++)
                    {
                        if (isPlayerAlive[i])
                        {
                            playerScores[i]++;
                            // see if a player has won
                            if (playerScores[i] >= maxScore)
                            {
                                someoneWon = true;
                            }
                        }
                    }
                    Debug.Log(endRoundScreen);
                    StartCoroutine(endRoundScreen.DisplayEndRoundUI());
                }
                // manage the hats

                if (ending)
                {
                    ending = false;
                    firstPass = true;
                    for (int i = 0; i < playersReady; i++)
                    {
                        if (!isPlayerAlive[i])
                        {
                            isPlayerAlive[i] = true;
                        }
                    }


                    // if someone has won, go to the end screen, else reset the game arena
                    GameObject playerManager = GameObject.Find("PlayerManager");
                    PlayerInput[] playerInputs = playerManager.GetComponentsInChildren<PlayerInput>();
                    if (!someoneWon && currRound < 3)
                    {


                        // activate playerinput and reset the scene
                        foreach (PlayerInput playerInput in playerInputs)
                        {
                            // move each player to their respective spawn point
                            CharacterController characterController = playerInput.GetComponent<CharacterController>();
                            characterController.enabled = false;
                            playerInput.transform.position = playerSpawns[playerInput.playerIndex].position;
                            playerInput.transform.rotation = playerSpawns[playerInput.playerIndex].rotation;
                            characterController.enabled = true;
                            playerInput.ActivateInput();

                            // get player controller component
                            PlayerController playerController = playerInput.GetComponent<PlayerController>();
                            // set the tag to player
                            playerController.gameObject.tag = "Player";
                        }
                        ManageHats();
                        currRound++;
                        PlayerController[] playerControllers = playerManager.GetComponentsInChildren<PlayerController>();
                        // enable the player indicators
                        foreach (PlayerController playerController in playerControllers)
                        {
                            playerController.EnableIndicator();
                        }
                        SwitchScene();
                    }
                    else
                    {
                        endScreen = true;
                        SceneManager.LoadScene("EndScene");
                    }
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
        GameObject playerManager = GameObject.Find("PlayerManager");
        PlayerInputManager inputManager = playerManager.GetComponent<PlayerInputManager>();
        inputManager.DisableJoining();
        yield return new WaitForSeconds(fade.TimeToFade);
        fadeing = false;

    }

    public int GetPlayerIndexFromInput(PlayerInput playerInput)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (playerInput == players[i])
            {
                return i;
            }
        }

        return -1;
    }

    public void SwitchScene()
    {
        SceneManager.LoadScene("NewMap");
        trainCalled = false;
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

    public void EndRound()
    {
        GameObject playerManager = GameObject.Find("PlayerManager");
        PlayerController[] playerControllers = playerManager.GetComponentsInChildren<PlayerController>();

        // Record the number of hats
        CountHats();

        // Remove the player indicators
        foreach (PlayerController playerController in playerControllers)
        {
            playerController.DisableIndicator();
        }

        // kill all players
        for (int i = 0; i < numPlayers; i++)
        {
            playerControllers[i].GetComponent<PlayerController>().KillPlayer();

        }
    }

    public void CountHats()
    {
        GameObject playerManager = GameObject.Find("PlayerManager");
        PlayerController[] playerControllers = playerManager.GetComponentsInChildren<PlayerController>();

        currWinner = 0;
        int maxHats = 0;

        // Iterate through each player, record the player with the most hats, and kill everyone else
        for (int i = 0; i < numPlayers; i++)
        {
            if (playerControllers[i].GetComponentInChildren<HatStack>().getNumHats() > maxHats)
            {
                maxHats = playerControllers[i].GetComponentInChildren<HatStack>().getNumHats();
                currWinner = i;
            }
            playerRoundHats[i] = playerControllers[i].GetComponentInChildren<HatStack>().getNumHats();
            playerHats[i] += playerControllers[i].GetComponentInChildren<HatStack>().getNumHats();
        }
    }


    public void StartGame()
    {
        // Add logic to start the game here
        // Load your game scene or perform any necessary initialization.
        currentScene = SceneManager.GetActiveScene();


        // iterate through the players and set the spawn points
        for (int i = 0; i < numPlayers; i++)
        {
            Debug.Log("CHANGING POISITON");
            CharacterController characterController = players[i].GetComponent<CharacterController>();
            characterController.enabled = false;
            players[i].transform.position = playerSpawns[i].position;
            characterController.enabled = true;
            Debug.Log("DROPPING ITEMS");
            PlayerController playerController = players[i].GetComponent<PlayerController>();
            playerController.DropObject();
        }

    }
}
