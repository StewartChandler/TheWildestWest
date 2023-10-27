using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ThrowableObject : MonoBehaviour
{
    public enum State {
        Prop = 0,
        Held = 1,
        Thrown = 2,
    };

    [SerializeField]
    private float hatReq;
    public State state { get; private set; }  = State.Prop;

    private Transform target = null;
    private Rigidbody rb;
    private Vector3 pickUpOffset = Vector3.Normalize(new Vector3(1.0f, 2f, 0.0f));
    private static int throwableMask = 8; // Throwable
    private float objMass;
    private float distAway;
    private float holderDist;
    private TrailRenderer trail;


    public static ThrowableObject getClosestAvailableObj(Vector3 point, int numHats, float range) {
        Collider[] colliders = Physics.OverlapSphere(point, range, throwableMask);

        float closestDistance = float.MaxValue;
        ThrowableObject closestThrowable = null;

        foreach (Collider collider in colliders)
        {
            ThrowableObject tObj = collider.gameObject.GetComponent<ThrowableObject>();
            if (tObj == null)
            {
                tObj = collider.gameObject.GetComponentInParent<ThrowableObject>();
            }
            if (tObj == null || tObj.state == State.Held) {
                continue;
            }

            float distance = Vector3.Distance(collider.ClosestPoint(point), point);
            if (distance < closestDistance)
            {
                if (true /*tObj.hatReq <= numHats + 1*/)
                {
                    closestThrowable = tObj;
                    closestDistance = distance;
                }
            }
        }

        return closestThrowable;
    }

    public void throwObject(Vector3 dir, float throwingSpeed)
    {
        rb.position = new Vector3(rb.position.x, target.position.y, rb.position.z);
        rb.mass = objMass;
        rb.useGravity = false;
        rb.velocity = (dir * throwingSpeed); // Adjust the throw force as needed.
        state = State.Thrown;

        if (trail != null)
        {
            trail.enabled = true;
        }
    }

    public void dropObject()
    {
        rb.mass = objMass;
        rb.useGravity = true;
        state = State.Prop;
        target = null;
        if (trail != null) { trail.enabled = false; }
    }

    public void pickupObject(Transform holder, float holderRadius)
    {
        target = holder;
        state = State.Held;
        objMass = rb.mass;
        rb.mass = 0;
        rb.useGravity = false;
        if (trail != null) { trail.enabled = false; }
        // Debug.Log(distAway);
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        trail = GetComponentInChildren<TrailRenderer>();

        if (trail != null)
        {
            trail.enabled = false;
        }

        // makes the object be futher away for larger objects
        distAway = 0;
        foreach (Collider collider in GetComponents<Collider>())
        {
            distAway = Mathf.Max(distAway, 0.5f * (new Vector2(collider.bounds.size.x, collider.bounds.size.y)).magnitude);
        }
        foreach (Collider collider in GetComponentsInChildren<Collider>())
        {
            distAway = Mathf.Max(distAway, 0.5f * (new Vector2(collider.bounds.size.x, collider.bounds.size.y)).magnitude);
        }
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case State.Prop:
                break;
            case State.Held:
                float farness = holderDist + distAway + 2.0f;

                // Calculate the desired position based on player's position and forward direction.
                Vector3 desiredPosition = target.position + farness * (target.forward * pickUpOffset.z + target.right * pickUpOffset.x) + target.up * pickUpOffset.y;

                // Lerp the object's position to the desired position for smooth movement.
                transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.fixedDeltaTime * 10f);

                // Match the rotation of the picked-up object to the player's rotation.
                transform.rotation = target.rotation;
                break;
            case State.Thrown:
                if (rb.velocity.magnitude < 0.1f)
                {
                    state = State.Prop;
                    target = null;
                    rb.useGravity = true;
                if (trail != null) { trail.enabled = false; }
                }

                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject collisionObject = collision.gameObject;
        if (state == State.Thrown && collision.gameObject.tag == "Player")
        {
            if (collision.gameObject != target.gameObject)
            {
                state = State.Prop;
                collision.gameObject.GetComponent<PlayerController>().takeDamage();
                target = null;
                rb.useGravity = true;

                if (trail != null) { trail.enabled = false; }
            } else { 
                rb.useGravity = true;
            }
        } else if (state == State.Thrown || state == State.Prop) { 
                rb.useGravity = true;
        }
    }
}
