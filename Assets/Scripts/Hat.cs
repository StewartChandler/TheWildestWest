using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hat : MonoBehaviour
{
    private Rigidbody rb;

    private enum State
    {
        Atop,
        Launched,
        Collectable
    }

    private State state;

    private Vector3 pos;
    private Vector3 scale;

    private float timeStill = 0.0f;
    private float timeCollectable = 0.0f;

    private Collider[] colliders;
    private Renderer[] renderers;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        colliders = GetComponentsInChildren<Collider>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void launch()
    {
        rb.isKinematic = false;
        rb.useGravity = true;
        Vector3 dir = transform.rotation * Vector3.up;
        rb.AddForce(500.0f * dir);
        rb.angularVelocity = Random.onUnitSphere;

        foreach (Collider c in colliders)
        {
            c.isTrigger = false;
            c.enabled = true;
        }

        state = State.Launched;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Atop:
                break;
            case State.Launched:
                if (rb.velocity.magnitude < 0.001f || rb.angularVelocity.magnitude < 0.001f)
                {
                    timeStill += Time.fixedDeltaTime;

                    if (timeStill > 1.0f)
                    {
                        pos = transform.localPosition;
                        scale = transform.localScale;

                        state = State.Collectable;

                        rb.isKinematic = true;
                        rb.useGravity = false;

                        timeCollectable = 0.0f;
                        timeStill = 0.0f;

                        foreach (Collider c in colliders)
                        {
                            c.isTrigger = true;
                        }
                    }
                }
                else
                {
                    timeStill = 0.0f;
                }
                break;
            case State.Collectable:
                timeCollectable += Time.fixedDeltaTime;

                const float transitionTime = 1.0f;
                float t = timeCollectable / transitionTime;

                float scalingFactor = Mathf.SmoothStep(1.0f, 0.7f, t);
                float heightFactor = Mathf.SmoothStep(0.0f, 1.0f, t);

                float height = 0.5f + 0.3f * Mathf.Sin(timeCollectable / Mathf.PI * 0.5f);

                float rotationSpeed = 72.0f * Mathf.Min(1.0f, t);

                transform.localEulerAngles += new Vector3(0.0f, rotationSpeed * Time.fixedDeltaTime, 0.0f);
                transform.localScale = scale * scalingFactor;
                transform.localPosition = pos + new Vector3(0.0f, heightFactor * height, 0.0f);

                if (timeCollectable > 3.0f) {
                    if (timeCollectable > 5.0f) Destroy(gameObject);

                    foreach (Renderer renderer in renderers)
                    {
                        renderer.enabled = (2.0f * timeCollectable - Mathf.Floor(2.0f * timeCollectable)) > 0.5f;
                    }
                }
                break;
        };


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HatStack hs = other.gameObject.GetComponentInChildren<HatStack>();

            if (hs != null && state == State.Collectable)
            {
                transform.localScale = scale;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;

                timeCollectable = 0.0f;
                timeStill = 0.0f;
                state = State.Atop;
                foreach (Collider c in colliders)
                {
                    c.enabled = false;
                }
                foreach (Renderer renderer in renderers)
                {
                    renderer.enabled = true;
                }

                hs.pushHat(gameObject);
            }
        }
    }
}
