using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerReadyUp : MonoBehaviour
{
    // create a private variable to store ready state
    public bool ready;
    public Material redMaterial;
    public Material greenMaterial;

    private GameManager gameManager;

    Transform spawnPlatform;
    Scene currentScene;
    private bool isRotatingNotReady = false;
    private bool isRotatingReady = false;


    private float rotationSpeed = 800f;
    private Quaternion initialRotationReady = Quaternion.Euler(0f, 0f, 0f);
    private Quaternion initialRotationNotReady = Quaternion.Euler(0f, 180f, 0f);
    private Quaternion finalRotation = Quaternion.Euler(0f, 360f, 0f);

    private TextMeshProUGUI readyText;
    Color newColor;


    void Start()
    {
        currentScene = SceneManager.GetActiveScene();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        spawnPlatform = FindStartPlatform();
        // change rotation to face away from the camera
        spawnPlatform.rotation = initialRotationReady;
        isRotatingReady = true;
        ready = false;
        ColorUtility.TryParseHtmlString("#005500", out newColor);
        readyText = spawnPlatform.Find("Canvas").Find("ClickToJoin").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (currentScene.name == "PlayerSelect" || currentScene.name == "PlayerSelectMap")
        {
            if (isRotatingNotReady)
            {
                // Interpolate the rotation from the initialRotation to Quaternion.Euler(0f, 180f, 0f)
                spawnPlatform.rotation = Quaternion.RotateTowards(spawnPlatform.rotation, finalRotation, rotationSpeed * Time.deltaTime);

                // Check if the rotation has reached Quaternion.Euler(0f, 180f, 0f)
                if (Quaternion.Angle(spawnPlatform.rotation, finalRotation) < 0.1f)
                {
                    // Snap to the final rotation and stop rotating
                    spawnPlatform.rotation = initialRotationReady;
                    isRotatingNotReady = false;
                }
            }
            if (isRotatingReady)
            {
                // Interpolate the rotation from the initialRotation to Quaternion.Euler(0f, 180f, 0f)
                spawnPlatform.rotation = Quaternion.RotateTowards(spawnPlatform.rotation, initialRotationNotReady, rotationSpeed * Time.deltaTime);

                // Check if the rotation has reached Quaternion.Euler(0f, 180f, 0f)
                if (Quaternion.Angle(spawnPlatform.rotation, initialRotationNotReady) < 0.1f)
                {
                    // Snap to the final rotation and stop rotating
                    spawnPlatform.rotation = initialRotationNotReady;
                    isRotatingReady = false;
                }
            }
        }
    }

    public void OnReady(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (currentScene.name == "PlayerSelect" || currentScene.name == "PlayerSelectMap")
            {
                ready = !ready;
                if (!ready)
                {
                    // Change the object's material to red when touched by the player.
                    // Start the rotation from the initialRotation
                    spawnPlatform.rotation = initialRotationReady;
                    isRotatingReady = true;
                    gameManager.playersReady--;

                }
                else
                {
                    // Change the object's material to green when touched by the player.

                    spawnPlatform.rotation = initialRotationNotReady;
                    isRotatingNotReady = true;
                    gameManager.playersReady++;
                    readyText.color = newColor;
                    readyText.text = "Ready!";
                }
            }
        }
    }
    // Function that finds the nearest object with tag "PlayerSpawn"
    Transform FindStartPlatform()
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
        // return the transform of closeststatrtplatform
        Debug.Log(closestStartPlatform);
        return closestStartPlatform.transform;
    }

}