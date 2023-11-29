using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    private Vector3 offset = new Vector3(0, 20, -20);
    public Vector3 offsetIncrease = new Vector3(0, 10, -10);
    public GameObject player;
    private GameObject[] players;
    public float smoothSpeed = 5f; // Adjust this value to control the smoothness
    public float furthestDistance = 70f;

    private GameManager gameManager;


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        players = GameObject.FindGameObjectsWithTag("Player");
        transform.rotation = Quaternion.Euler(55f, 0f, 0f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //gameObject.transform.position = player.transform.position;
        //gameObject.transform.Translate(Offset);
        if (gameManager.firstPass)
        {

            Vector3 targetPosition = CalculateCameraTargetPosition();
            targetPosition += offset;

            // Camera up down, back forward (zoom) distance
            float maxDistance = CalculateMaxPlayerDistance(targetPosition);
            float distanceWeight = maxDistance / furthestDistance;

            targetPosition += offsetIncrease * distanceWeight;

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
                Vector3 positionToAdd = p.transform.position;
                positionToAdd.y = 0;
                centerPosition += positionToAdd;
                livePlayers += 1;
            }
        }

        if (livePlayers != 0)
        {
            return centerPosition / livePlayers;
        }

        return offset;
    }

    float CalculateMaxPlayerDistance(Vector3 centerPosition)
    {
        float xMaxFromCenter = 0f;
        float zMaxFromCenter = 0f;

        for (int i = 0; i < players.Length; i++)
        {
            float x = Mathf.Abs(centerPosition.x - players[i].transform.position.x);
            float z = Mathf.Abs(centerPosition.z - players[i].transform.position.z);

            xMaxFromCenter = Mathf.Max(x, xMaxFromCenter);
            zMaxFromCenter = Mathf.Max(z, zMaxFromCenter);
        }

        Vector3 player1 = new Vector3(centerPosition.x - xMaxFromCenter, 0, centerPosition.z - zMaxFromCenter);
        Vector3 player2 = new Vector3(centerPosition.x + xMaxFromCenter, 0, centerPosition.z + zMaxFromCenter);

        float maxDistance = Vector3.Distance(player1, player2);

        return maxDistance;
    }
}
