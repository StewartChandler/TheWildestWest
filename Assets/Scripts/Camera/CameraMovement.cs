using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    private Vector3 offset = new Vector3(0, 18, -13);
    private Vector3 offsetIncrease = new Vector3(0, 14, -14);
    private GameObject[] players;
    private float smoothSpeed = 5f; // Adjust this value to control the smoothness
    private float furthestDistance = 40f;

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
        if (true) // gameManager.firstPass)
        {
            //find the two fake players
            bool playerFound = false;
            Vector3 bigPlayerPos = Vector3.zero;
            Vector3 smallPlayerPos = Vector3.zero;

            foreach (GameObject p in players)
            {
                PlayerInput playerInput = p.GetComponent<PlayerInput>();

                if (gameManager.isPlayerAlive[playerInput.playerIndex])
                {
                    if (!playerFound)
                    {
                        bigPlayerPos = p.transform.position;
                        smallPlayerPos = p.transform.position;
                        playerFound = true;
                    } else
                    {
                        if (p.transform.position.x < smallPlayerPos.x)
                        {
                            smallPlayerPos.x = p.transform.position.x;
                        } else if (p.transform.position.x > bigPlayerPos.x)
                        {
                            bigPlayerPos.x = p.transform.position.x;
                        }
                        if (p.transform.position.z < smallPlayerPos.z)
                        {
                            smallPlayerPos.z = p.transform.position.z;
                        } else if (p.transform.position.z > bigPlayerPos.z)
                        {
                            bigPlayerPos.z = p.transform.position.z;
                        }
                    }
                }
            }

            // set the y components to 0
            bigPlayerPos.y = 0;
            smallPlayerPos.y = 0;

            //Find the middle between the two player postions and add the offset to get the camera target position
            Vector3 targetPosition = bigPlayerPos + smallPlayerPos;
            targetPosition = targetPosition / 2;
            targetPosition += offset;

            // Camera up down, back forward (zoom) distance
            float maxDistance = Vector3.Distance(bigPlayerPos, smallPlayerPos);
            Debug.Log(maxDistance);
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
