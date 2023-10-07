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

    Renderer objectRenderer;

    public void OnReady(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ready = !ready;
            if (!ready)
            {
                // Change the object's material to red when touched by the player.
                objectRenderer.material = redMaterial;
            }
            else
            {
                // Change the object's material to green when touched by the player.
                objectRenderer.material = greenMaterial;
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the player touches an object with a specific tag (e.g., "Interactable").
        if (collision.gameObject.CompareTag("SpawnPlatform"))
        {
            objectRenderer = collision.gameObject.GetComponent<Renderer>();

            if (objectRenderer != null)
            {
                if (!ready)
                {
                    // Change the object's material to red when touched by the player.
                    objectRenderer.material = redMaterial;
                }
                else
                {
                    // Change the object's material to green when touched by the player.
                    objectRenderer.material = greenMaterial;
                }
            }
        }
    }
}