using System.Collections;
using System.Collections.Generic;
using System.Resources;
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
    private Vector3 pickUpOffset = new Vector3(1.0f, 0.5f, 0.0f);
    private static int throwableMask = 8; // Throwable
    private float objMass;
    private float distAway;
    private float holderDist;
    private float yOffset;
    private TrailRenderer trail;
    private float throwtime;
    public bool respawn = true;
    private Vector3 spawnPos;
    private Vector3 spawnOffset = new Vector3(0.0f, 20.0f);

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

    public void resetState() {
        state = State.Prop;
        target = null;
        rb.useGravity = true;
        rb.mass = objMass;

        if (trail != null) { trail.enabled = false; }
    }

    Vector3 calcDesiredPos()
    {

        float farness = holderDist + distAway + 0.5f;

        // Calculate the desired position based on player's position and forward direction.
        return target.position + farness * (target.forward * pickUpOffset.z + target.right * pickUpOffset.x) + target.up * (pickUpOffset.y + yOffset);
    }

    public void throwObject(Vector3 dir, float throwingSpeed)
    {

        // instead just throw from desired position
        Vector3 desiredPos = calcDesiredPos();
        rb.position = new Vector3(desiredPos.x, 0.5f + yOffset, desiredPos.z);

        rb.mass = objMass;
        rb.useGravity = false;
        rb.velocity = (dir * throwingSpeed); // Adjust the throw force as needed.
        state = State.Thrown;
        throwtime = 0;

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
        // dist away should be 2-dimensional, only a problem with railroad tracks for now though
        distAway = 0;
        yOffset = 0;
        foreach (Collider collider in GetComponents<Collider>())
        {
            distAway = Mathf.Max(distAway, 0.5f * (new Vector2(collider.bounds.size.x, collider.bounds.size.y)).magnitude);
            yOffset = Mathf.Max(yOffset, collider.bounds.extents.y);
        }
        foreach (Collider collider in GetComponentsInChildren<Collider>())
        {
            distAway = Mathf.Max(distAway, 0.5f * (new Vector2(collider.bounds.size.x, collider.bounds.size.y)).magnitude);
            yOffset = Mathf.Max(yOffset, collider.bounds.extents.y);
        }

        spawnPos = transform.position + spawnOffset;
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case State.Prop:
                break;
            case State.Held:
                // Calculate the desired position based on player's position and forward direction.
                Vector3 desiredPosition = calcDesiredPos();

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
                    break;
                }

                throwtime += Time.fixedDeltaTime;

                if (trail != null)
                {
                    // ajdust trail scale based on speed
                    trail.widthMultiplier = Mathf.SmoothStep(0f, 1f, rb.velocity.magnitude / 15f);

                    // remove trail after some time
                    if (throwtime >= 20f) { trail.enabled = false; }
                }
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject collisionObject = collision.gameObject;
        if (collision.gameObject.tag == "DeathZone")
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        } else if (collision.gameObject.tag == "ItemRespawnPlane")
        {
            if (respawn == true)
            {
                resetState();
                transform.position = spawnPos;
                rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
            } else
            {
                Destroy(gameObject);
            }
        } else if (state == State.Thrown && collision.gameObject.tag == "Player")
        {
            if (collision.gameObject != target.gameObject)
            {
                state = State.Prop;
                collision.gameObject.GetComponent<PlayerController>().takeDamage(target.position - collision.gameObject.transform.position);
                target = null;
                rb.mass = objMass;
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
