using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerReadyUp : MonoBehaviour
{
    // create a private variable to store ready state
    public bool ready = true;
    public Material redMaterial;
    public Material greenMaterial;

    Renderer spawnPlatform;

    public void OnReady(InputAction.CallbackContext context)
    {
        spawnPlatform = FindStartPlatform();
        if (context.performed)
        {
            ready = !ready;
            if (!ready)
            {
                // Change the object's material to red when touched by the player.
                spawnPlatform.material = redMaterial;
            }
            else
            {
                // Change the object's material to green when touched by the player.
                spawnPlatform.material = greenMaterial;
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