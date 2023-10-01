using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInitialization : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform player1Spawn;
    public Transform player2Spawn;
    public Transform player3Spawn;
    public Transform player4Spawn;

    private void Awake()
    {

        // Instantiate player input for Player 1 (KeyboardRight)
        var player1 = Instantiate(playerPrefab, player1Spawn.position, player1Spawn.rotation, gameObject.transform);
        player1.GetComponent<PlayerInput>().SwitchCurrentControlScheme("KeyboardRight", Keyboard.current);
        player1.name = "Player1";

        // Instantiate player input for Player 2 (KeyboardLeft)
        var player2 = Instantiate(playerPrefab, player2Spawn.position, player2Spawn.rotation, gameObject.transform);
        player2.GetComponent<PlayerInput>().SwitchCurrentControlScheme("KeyboardLeft", Keyboard.current);
        player2.name = "Player2";

    }
}