using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class DeathPlane : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        // Check if the colliding object has the "Player" tag
        if (collision.gameObject.CompareTag("Player"))
        {

            // if scene is PlayerSelectMap, respawn player at spawn point
            if (SceneManager.GetActiveScene().name == "PlayerSelectMap")
            {
                CharacterController characterController = collision.gameObject.GetComponent<CharacterController>();
                // disable player controller
                characterController.enabled = false;
                // move player to 0,10,0
                collision.gameObject.transform.position = new Vector3(0f, 10f, 0f);
                // enable player controller
                characterController.enabled = true;

            }
            else
            {
                Debug.Log("Player has fallen off the map!");
                PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();


                PlayerInput playerInput = collision.gameObject.GetComponent<PlayerInput>();
                StatsManager.instance.TimesFallen(playerInput.playerIndex);

                // playerController.KillPlayer();
                playerController.takeDamage(-0.4f * collision.transform.position + new Vector3(0f, 10f));
                playerController.RespawnPlayer();
            }
        }
    }
}
