using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 Offset = new Vector3(0, 10, -15);
    public GameObject player;
    private GameObject[] players;
    public float smoothSpeed = 5f; // Adjust this value to control the smoothness


    // Start is called before the first frame update
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //gameObject.transform.position = player.transform.position;
        //gameObject.transform.Translate(Offset);

        Vector3 targetPosition = CalculateCameraTargetPosition();
        targetPosition += Offset;

        // Use Lerp to smoothly interpolate the camera's position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }

    Vector3 CalculateCameraTargetPosition()
    {
        Vector3 centerPosition = Vector3.zero;

        foreach (GameObject p in players)
        {
            centerPosition += p.transform.position;
        }

        return centerPosition / players.Length;
    }
}
