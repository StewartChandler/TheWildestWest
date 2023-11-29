using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    private Vector3 offset = new Vector3(0, 20, -20);
    public Vector3 offsetIncrease = new Vector3(0, 15, -15);
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
            float maxDistance = CalculateMaxPlayerDistance();
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
                if (!gameManager.isPlayerAlive[playerInput1.playerIndex] || !gameManager.isPlayerAlive[playerInput2.playerIndex])
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
