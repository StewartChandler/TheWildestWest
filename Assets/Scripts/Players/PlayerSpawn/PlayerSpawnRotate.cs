using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnRotate : MonoBehaviour
{
    private float rotationSpeed = 20.0f;

    private Vector3 player1RotationDirection;
    private Vector3 player2RotationDirection;
    private Vector3 player3RotationDirection;
    private Vector3 player4RotationDirection;
    Scene currentScene;

    // Update is called once per frame
    void Start()
    {
        // Create a randomized rotation direction
        player1RotationDirection = new Vector3(Random.Range(-0.6f, 0.6f), Random.Range(-1f, 1f), Random.Range(-0.4f, 0.4f));
        player2RotationDirection = new Vector3(Random.Range(-0.6f, 0.6f), Random.Range(-1f, 1f), Random.Range(-0.4f, 0.4f));
        player3RotationDirection = new Vector3(Random.Range(-0.6f, 0.6f), Random.Range(-1f, 1f), Random.Range(-0.4f, 0.4f));
        player4RotationDirection = new Vector3(Random.Range(-0.6f, 0.6f), Random.Range(-1f, 1f), Random.Range(-0.4f, 0.4f));
        currentScene = SceneManager.GetActiveScene();

    }
    void FixedUpdate()
    {
        // Check the name of the player to see if it is player1, 2, or 4
        // If it is, rotate each player differently
        if (currentScene.name == "PlayerSelect")
        {

            if (gameObject.name == "Player1")
            {
                transform.Rotate(player1RotationDirection * rotationSpeed * Time.deltaTime);
            }
            else if (gameObject.name == "Player2")
            {
                transform.Rotate(player2RotationDirection * rotationSpeed * Time.deltaTime);
            }
            else if (gameObject.name == "Player3")
            {
                transform.Rotate(player3RotationDirection * rotationSpeed * Time.deltaTime);
            }
            else if (gameObject.name == "Player4")
            {
                transform.Rotate(player4RotationDirection * rotationSpeed * Time.deltaTime);
            }
        }
    }

}
