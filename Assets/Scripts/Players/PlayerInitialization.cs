using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInitialization : MonoBehaviour
{
    public GameObject playerPrefab;

    private void Awake()
    {
        // Instantiate player input for Player 1 (KeyboardRight)
        var player1 = PlayerInput.Instantiate(playerPrefab, controlScheme: "KeyboardRight", playerIndex: 0, pairWithDevice: Keyboard.current);
        player1.transform.position = new Vector3(1.0f, 0.0f, 1.0f);
        player1.name = "Player1";


        // Instantiate player input for Player 2 (KeyboardLeft)
        var player2 = PlayerInput.Instantiate(playerPrefab, controlScheme: "KeyboardLeft", playerIndex: 1, pairWithDevice: Keyboard.current);
        player2.transform.position = new Vector3(-1.0f, 0.0f, -1.0f);
        player2.name = "Player2";

    }
}