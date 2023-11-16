using UnityEngine;

public class Icicle : ThrowableObject
{
    public float freezeDuration = 2.0f;
    public GameObject icicleBreakEffect; // Reference to a particle effect for icicle break

    // Override the OnCollisionEnter method to handle icicle-specific behavior on collision
    private void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision); // Call the base method to handle common collision behavior

        if (state == State.Thrown && collision.gameObject.tag == "Player")
        {
            if (collision.gameObject != target.gameObject)
            {
                collision.gameObject.GetComponent<PlayerController>().freezePlayer(freezeDuration);

                // Break the icicle
                BreakIcicle();
            }
        }
    }

    // Method to break the icicle
    private void BreakIcicle()
    {
        Debug.Log("Icicle broken");

        // // Play a particle effect for icicle break
        // if (icicleBreakEffect != null)
        // {
        //     Instantiate(icicleBreakEffect, transform.position, Quaternion.identity);
        // }

        // Destroy the icicle object
        Destroy(gameObject);
    }
}
