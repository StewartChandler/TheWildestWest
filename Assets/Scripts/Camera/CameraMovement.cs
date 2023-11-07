using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    private Vector3 Offset = new Vector3(0, 20, -25);
    public GameObject player;
    private GameObject[] players;
    public float smoothSpeed = 5f; // Adjust this value to control the smoothness

    private GameManager gameManager;


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        players = GameObject.FindGameObjectsWithTag("Player");
        transform.rotation = Quaternion.Euler(45f, 0f, 0f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //gameObject.transform.position = player.transform.position;
        //gameObject.transform.Translate(Offset);
        if (gameManager.firstPass)
        {

            Vector3 targetPosition = CalculateCameraTargetPosition();
            targetPosition += Offset;

            // Camera up down, back forward (zoom) distance
            float maxDistance = CalculateMaxPlayerDistance();

            float upOffset = Mathf.Lerp(16f, 26f, maxDistance / 70f); // v1 = v2 * 0.8
            float backOffset = Mathf.Lerp(-25.6f, -32f, maxDistance / 70f); // v1 = v2 * 0.8

            targetPosition.y += upOffset - 20;
            targetPosition.z += backOffset + 25;

            // Use Lerp to smoothly interpolate the camera's position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
    }

    Vector3 CalculateCameraTargetPosition()
    {
        Vector3 centerPosition = Vector3.zero;
        int livePlayers = 0;

        foreach (GameObject p in players)
        {
            PlayerInput playerInput = p.GetComponent<PlayerInput>();

            if (gameManager.isPlayerAlive[playerInput.playerIndex])
            {
                centerPosition += p.transform.position;
                livePlayers += 1;
            }
        }

        if (livePlayers != 0)
        {
            return centerPosition / livePlayers;
        }

        return Offset;
    }

    float CalculateMaxPlayerDistance()
    {
        float maxDistance = 0f;

        for (int i = 0; i < players.Length; i++)
        {
            for (int j = i + 1; j < players.Length; j++)
            {
                PlayerInput playerInput1 = players[i].GetComponent<PlayerInput>();
                PlayerInput playerInput2 = players[j].GetComponent<PlayerInput>();

                // don't calculate distance when one of the players aren't alive
                if (gameManager.isPlayerAlive[playerInput1.playerIndex] || gameManager.isPlayerAlive[playerInput2.playerIndex])
                {
                    continue;
                }

                float playerDistance = Vector3.Distance(players[i].transform.position, players[j].transform.position);
                maxDistance = Mathf.Max(maxDistance, playerDistance);
            }
        }

        return maxDistance;
    }
}
