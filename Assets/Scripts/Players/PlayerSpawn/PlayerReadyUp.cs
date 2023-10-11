using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerReadyUp : MonoBehaviour
{
    // create a private variable to store ready state
    public bool ready;
    public Material redMaterial;
    public Material greenMaterial;

    private GameManager gameManager;

    Renderer spawnPlatform;
    Scene currentScene;

    void Start()
    {
        currentScene = SceneManager.GetActiveScene();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        spawnPlatform = FindStartPlatform();
        spawnPlatform.material = redMaterial;
        ready = false;
    }

    public void OnReady(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (currentScene.name == "PlayerSelect")
            {
                ready = !ready;
                if (!ready)
                {
                    // Change the object's material to red when touched by the player.
                    spawnPlatform.material = redMaterial;
                    gameManager.playersReady--;

                }
                else
                {
                    // Change the object's material to green when touched by the player.
                    gameManager.playersReady++;
                    spawnPlatform.material = greenMaterial;
                }
            }
        }
    }
    // Function that finds the nearest object with tag "PlayerSpawn"
    Renderer FindStartPlatform()
    {
        GameObject[] startPlatforms = GameObject.FindGameObjectsWithTag("SpawnPlatform");
        GameObject closestStartPlatform = null;
        float closestDistance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject startPlatform in startPlatforms)
        {
            Vector3 diff = startPlatform.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < closestDistance)
            {
                closestStartPlatform = startPlatform;
                closestDistance = curDistance;
            }
        }
        return closestStartPlatform.GetComponent<Renderer>();
    }

}