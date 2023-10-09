using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnRotate : MonoBehaviour
{
    private float rotationSpeed = 20.0f;

    private Vector3 rotationDirection = new Vector3(0.6f, 1f, 0.4f);

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(rotationDirection * rotationSpeed * Time.deltaTime);
    }
}
