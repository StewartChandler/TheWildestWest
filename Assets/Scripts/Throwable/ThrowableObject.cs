using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using Unity.VisualScripting;
using UnityEngine;

public class ThrowableObject : MonoBehaviour
{
    public enum State
    {
        Prop = 0,
        Held = 1,
        Thrown = 2,
    };

    [SerializeField]
    private float hatReq;
    public State state { get; protected set; } = State.Prop;

    protected Transform target = null;
    protected Rigidbody rb;
    private Vector3 pickUpOffset = new Vector3(1.0f, 0.5f, 0.0f);
    private static int throwableMask = 8; // Throwable
    protected float objMass;
    private float distAway;
    private float holderDist;
    private float yOffset;
    protected TrailRenderer trail;
    protected float throwtime;

    private Material highlightMaterial;
    private List<Material> originalMaterial = new List<Material>();
    private List<Renderer> objectRenderer = new List<Renderer>();

    public void activateHighlight(Color playerColor)
    {
        foreach (var item in objectRenderer)
        {
            item.material = highlightMaterial;
            item.material.color = playerColor;
        }
    }

    public void removeHighlight()
    {
        for(int i = 0; i < objectRenderer.Count; i++)
        {
            objectRenderer[i].material = originalMaterial[i];
        }
    }

    public void setUpMaterials()
    {
        // TODO: REFACTOR THIS, since alot of the prefabs have different places where the 
        highlightMaterial = Resources.Load<Material>("Highlight");
        if (highlightMaterial == null)
        {
            Debug.Log("mat is null");
        }
        var rend = GetComponent<Renderer>();
        if (rend != null)
        {
            objectRenderer.Add(rend);
        }
        foreach (var item in GetComponentsInChildren<Renderer>())
        {
            objectRenderer.Add(item);
        }

        foreach (var item in objectRenderer)
        {
            originalMaterial.Add(item.material);
        }

    }

    public bool respawn = true;
    protected Vector3 spawnPos;
    private Vector3 spawnOffset = new Vector3(0.0f, 20.0f);

    public static ThrowableObject getClosestAvailableObj(Vector3 point, int numHats, float range)
    {
        Collider[] colliders = Physics.OverlapSphere(point, range, throwableMask);

        float closestDistance = float.MaxValue;
        ThrowableObject closestThrowable = null;
        bool closestIsThrown = false;

        foreach (Collider collider in colliders)
        {
            ThrowableObject tObj = collider.gameObject.GetComponent<ThrowableObject>();
            if (tObj == null)
            {
                tObj = collider.gameObject.GetComponentInParent<ThrowableObject>();
            }
            if (tObj == null || tObj.state == State.Held)
            {
                continue;
            }

            bool tObjIsThrown = tObj.state == State.Thrown;

            float distance = Vector3.Distance(collider.ClosestPoint(point), point);
            if ((distance < closestDistance && tObjIsThrown == closestIsThrown) || (!closestIsThrown && tObjIsThrown))
            {
                if (true /*tObj.hatReq <= numHats + 1*/)
                {
                    closestThrowable = tObj;
                    closestDistance = distance;
                    closestIsThrown = tObjIsThrown;
                }
            }
        }

        return closestThrowable;
    }

    public virtual void resetState()
    {
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

    public virtual void throwObject(Vector3 dir, float throwingSpeed)
    {

        // instead just throw from desired position
        Vector3 desiredPos = calcDesiredPos();
        rb.position = new Vector3(desiredPos.x, 1.5f + yOffset, desiredPos.z);

        rb.mass = objMass;
        rb.useGravity = false;
        rb.velocity = (dir * throwingSpeed); // Adjust the throw force as needed.
        state = State.Thrown;
        throwtime = 0;

        if (trail != null)
        {
            trail.enabled = true;
            trail.Clear();
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

        setUpMaterials();

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

                // after some time since thrown re-enable gravity
                if (throwtime >= 3f) { rb.useGravity = true; }

                if (trail != null)
                {
                    // ajdust trail scale based on speed
                    trail.widthMultiplier = Mathf.SmoothStep(0f, 1f, rb.velocity.magnitude / 15f);

                    // remove thrown state after 10s if not done already
                    if (throwtime >= 10f) { resetState(); }
                }
                break;
        }
    }

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
                collision.gameObject.GetComponent<PlayerController>().takeDamage(target.position - collision.gameObject.transform.position);
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
}
