using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSceneTrain : MonoBehaviour
{
    public float delayInSeconds = 3f;
    public float velocityToAdd = 90f;
    private bool velocityAdded = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AddVelocityAfterDelay());
    }

    // Update is called once per frame
    
    IEnumerator AddVelocityAfterDelay()
    {
        yield return new WaitForSeconds(delayInSeconds);

        if (!velocityAdded)
        {
            Rigidbody rb = GetComponent<Rigidbody>();

            // If the Rigidbody component is present, add constant velocity
            if (rb != null)
            {
                rb.velocity += transform.right * -velocityToAdd;
            }
            velocityAdded = true;
        }
    }
}
