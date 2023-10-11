using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Highlight : MonoBehaviour
{
    public float pickupRange = 5f;
    public Material highlightMaterial; // Assign the highlight material in the Inspector.
    public Transform closestObject;
    public Material originalMaterial; // Store the original material of the closest object.
    public Collider closestCollider;

    private HatStack hatStack;

    Scene currentScene;

    private void Start()
    {
        hatStack = GetComponentInChildren<HatStack>();
        currentScene = SceneManager.GetActiveScene();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentScene.name != "PlayerSelect")
        {
            HighlightClosestObject();
        }
    }
    void HighlightClosestObject()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRange);

        float closestDistance = float.MaxValue;
        closestCollider = null;

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Throwable"))
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    float colMass = collider.GetComponent<Rigidbody>().mass;
                    if (colMass <= hatStack.getNumHats() + 1)
                    {
                        closestDistance = distance;
                        closestCollider = collider;
                    }
                }
            }
        }

        if (closestCollider == null)
        {
            if (closestObject != null)
            {
                closestObject.GetComponent<Renderer>().material = originalMaterial;
            }
        }

        if (closestCollider != null)
        {
            if (closestObject != null)
            {
                closestObject.GetComponent<Renderer>().material = originalMaterial;
            }

            // Retrieve the closest object
            closestObject = closestCollider.transform;

            // Store the original material of the closest object.
            originalMaterial = closestObject.GetComponent<Renderer>().material;

            // Highlight the closest object.
            closestObject.GetComponent<Renderer>().material = highlightMaterial;

        }
    }
}
