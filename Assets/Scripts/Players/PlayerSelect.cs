using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerSelect : MonoBehaviour
{
    // Create 4 public spawn platform objects
    public GameObject[] spawnPlatforms = new GameObject[4];
    private int numPlayers = 0;

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        SetPlayerPositionAndColor(playerInput.transform);
    }

    void SetPlayerPositionAndColor(Transform player)
    {
        // Set the player's position to 4 above the spawn platform
        player.position = spawnPlatforms[numPlayers].transform.position + new Vector3(0, 4, 0);

        // Increment the number of players
        numPlayers++;
    }
}
