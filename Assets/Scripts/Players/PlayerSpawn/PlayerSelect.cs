using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerSelect : MonoBehaviour
{
    // Create 4 public spawn platform objects
    public GameObject[] spawnPlatforms = new GameObject[4];
    private int numPlayers = 0;
    float scale = 0.0f;
    private float scaleSpeed = 2f;
    private bool isScaling = false;
    private Vector3 initialScale = new Vector3(0f, 0f, 0f);
    private Vector3 targetScale = new Vector3(0.8f, 0.8f, 0.8f);

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        SetPlayerPositionAndColor(playerInput.transform);
        if (!isScaling)
        {
            StartCoroutine(ScaleObject(playerInput.transform));
        }
    }

    void SetPlayerPositionAndColor(Transform player)
    {
        // Set the player's position to 4 above the spawn platform
        player.position = spawnPlatforms[numPlayers].transform.position + new Vector3(0f, 1.5f, 0f);

        // Increment the number of players
        numPlayers++;
    }
    private IEnumerator ScaleObject(Transform player)
    {
        isScaling = true;
        while (scale < 1.0f)
        {
            scale += scaleSpeed * Time.deltaTime;
            player.localScale = Vector3.Lerp(initialScale, targetScale, scale);
            yield return null;
        }
        isScaling = false;
    }
}
