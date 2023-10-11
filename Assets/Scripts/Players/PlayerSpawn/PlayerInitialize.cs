using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInitialize : MonoBehaviour
{
    // Start is called before the first frame update
    // List of player spawns
    public Transform[] playerSpawns = new Transform[4];
    public void changeSpawn()
    {
        // Find all objects of type player
        Debug.Log("Changing Spawn");
        PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();

        // change all the players to the correct spawn point
        for (int i = 0; i < playerInputs.Length; i++)
        {
            playerInputs[i].transform.position = playerSpawns[i].position;
            playerInputs[i].transform.rotation = playerSpawns[i].rotation;
        }

    }
}
