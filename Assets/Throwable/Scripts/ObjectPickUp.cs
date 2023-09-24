using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPickUp : MonoBehaviour
{
    public float pickupRange = 5f;
    private Transform pickedObject;
    private Rigidbody pickedRigidbody;
    private Vector3 pickUpOffset = new Vector3(0.5f, 0.5f, 0.5f);

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (pickedObject == null)
            {
                PickUpClosestObject();
            }
            else
            {
                ThrowObject();
            }
        }
        if (pickedObject != null)
        {
            // Calculate the desired position based on player's position and forward direction.
            Vector3 desiredPosition = transform.position + transform.forward * pickUpOffset.z + transform.right * pickUpOffset.x + transform.up * pickUpOffset.y;

            // Lerp the object's position to the desired position for smooth movement.
            pickedObject.position = Vector3.Lerp(pickedObject.position, desiredPosition, Time.deltaTime * 10f);

            // Match the rotation of the picked-up object to the player's rotation.
            pickedObject.rotation = transform.rotation;
        }
    }

    void PickUpClosestObject()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRange);

        float closestDistance = float.MaxValue;
        Collider closestCollider = null;

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Throwable"))
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCollider = collider;
                }
            }
        }

        if (closestCollider != null)
        {
            pickedObject = closestCollider.transform;
            pickedRigidbody = pickedObject.GetComponent<Rigidbody>();
            pickedRigidbody.isKinematic = true;

            // Move the object closer to the player.
            pickedObject.position = transform.position + pickUpOffset;
        }
    }

    void ThrowObject()
    {
        if (pickedObject != null)
        {
            pickedRigidbody.isKinematic = false;
            pickedRigidbody.velocity = (transform.forward * 20f); // Adjust the throw force as needed.
            pickedObject = null;
        }
    }
}

