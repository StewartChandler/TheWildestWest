using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class GameManager : MonoBehaviour
{
    public PlayerInput[] players = new PlayerInput[4];
    public int playersReady = 0;
    public int numPlayers = 0;
    private static GameManager instance;
    Scene currentScene;
    public Transform[] playerSpawns = new Transform[4];
    public GameObject playerPrefab;
    [SerializeField] private UnityEvent _onGameStart;


    private void Awake()
    {
        currentScene = SceneManager.GetActiveScene();
    }
    private void Update()
    {
        if (currentScene.name == "PlayerSelect")
        {
            if (numPlayers >= 1 && numPlayers == playersReady)
            {
                Debug.Log("Starting Game");
                _onGameStart.Invoke();

            }
        }
    }

    public void SwitchScene()
    {
        SceneManager.LoadScene("DemoArena");
    }



    public void StartGame()
    {
        // Add logic to start the game here
        // Load your game scene or perform any necessary initialization.
        currentScene = SceneManager.GetActiveScene();
        Debug.Log(currentScene.name);

        // // Find the playerSpawns object in the scene
        playerSpawns = GameObject.Find("Spawn Points").GetComponentsInChildren<Transform>();
        Debug.Log(playerSpawns.Length);

        if (numPlayers == 0 || numPlayers == 1)
        {
            // Instantiate player input for Player 1 (KeyboardRight)
            var player1 = Instantiate(playerPrefab, playerSpawns[1].position, playerSpawns[1].rotation, gameObject.transform);
            player1.GetComponent<PlayerInput>().SwitchCurrentControlScheme("KeyboardRight", Keyboard.current);
            players[0] = player1.GetComponent<PlayerInput>();
            player1.name = "Player1";

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
                players[i].transform.position = playerSpawns[i].position;
            }
        }


    }
}
