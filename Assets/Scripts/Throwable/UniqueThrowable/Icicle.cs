using UnityEngine;

public class Icicle : ThrowableObject
{
    public float freezeDuration = 5.0f;
    public GameObject icicleBreakEffect; // Reference to a particle effect for icicle break

    // Override the OnCollisionEnter method to handle icicle-specific behavior on collision
    protected void OnCollisionEnter(Collision collision)
    {
        GameObject collisionObject = collision.gameObject;
        if (collision.gameObject.tag == "DeathZone")
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        }
        else if (collision.gameObject.tag == "ItemRespawnPlane")
        {
            if (respawn == true)
            {
                resetState();
                transform.position = spawnPos;
                rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else if (state == State.Thrown && collision.gameObject.tag == "Player")
        {
            if (collision.gameObject != target.gameObject)
            {
                state = State.Prop;
                collision.gameObject.GetComponent<PlayerController>().takeDamage(target.position - collision.gameObject.transform.position, true);
                target = null;
                rb.mass = objMass;
                rb.useGravity = true;

                if (trail != null) { trail.enabled = false; }
            }
            else
            {
                rb.useGravity = true;
            }
        }
        else if (state == State.Thrown || state == State.Prop)
        {
            rb.useGravity = true;
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
