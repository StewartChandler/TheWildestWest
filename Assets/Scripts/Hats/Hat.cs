using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Hat : MonoBehaviour
{
    private Rigidbody rb;

    private static float scalingFactor = 1.5f;

    private enum State
    {
        Atop,
        Launched,
        Collectable
    }

    private State state;

    private Vector3 pos;
    // private Vector3 scale;

    private float timeStill = 0.0f;
    private float timeCollectable = 0.0f;

    private Collider[] colliders;
    private Renderer[] renderers;
    private Material hatOutline;
    Scene currentScene;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        colliders = GetComponentsInChildren<Collider>();
        renderers = GetComponentsInChildren<Renderer>();

        state = State.Atop;
        foreach (Collider c in colliders)
        {
            c.isTrigger = true;
            c.enabled = false;
        }
        currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "PlayerSelect")
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = false;
            }
        }

        transform.localScale = scalingFactor * Vector3.one;

        var mats = GetComponentInChildren<Renderer>().materials;
        if (mats.Length >= 2)
        {
            hatOutline = mats[1]; 
        }
        hatOutline.SetFloat("_Outline_Thickness", 0.0025f);
    }

    public void setHatColour(Color c)
    {
        var mats = GetComponentInChildren<Renderer>().materials;
        if (mats.Length >= 2)
        {
            mats[1].color = c;
        }
    }

    public void launch(Vector3 displ)
    {
        rb.isKinematic = false;
        rb.useGravity = true;
        Vector3 dir = displ;
        float dist = Vector3.Magnitude(dir);
        dir *= 1 / dist;
        dist = Mathf.Max(dist, 1);
        dir += Vector3.up;
        rb.AddForce(25.0f * dist * dir);
        rb.angularVelocity = Random.onUnitSphere;

        transform.localScale = scalingFactor * Vector3.one;

        transform.position += Vector3.up * 0.4f;

        foreach (Collider c in colliders)
        {
            c.isTrigger = false;
            c.enabled = true;
        }

        hatOutline.SetFloat("_Outline_Thickness", 0.01f);

        state = State.Launched;
    }

    private void FixedUpdate()
    {


        switch (state)
        {
            case State.Atop:
                if (currentScene.name != "PlayerSelect")
                {
                    foreach (Renderer renderer in renderers)
                    {
                        renderer.enabled = true;
                    }
                }
                break;
            case State.Launched:
                if (rb.velocity.magnitude < 0.001f || rb.angularVelocity.magnitude < 0.001f)
                {
                    timeStill += Time.fixedDeltaTime;

                    if (timeStill > 0.05f)
                    {
                        pos = transform.localPosition;
                        // scale = transform.localScale;

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

                // float scalingFactor = Mathf.SmoothStep(1.0f, 0.7f, t);
                float heightFactor = Mathf.SmoothStep(0.0f, 1.0f, t);

                float height = 0.5f + 0.3f * Mathf.Sin(timeCollectable / Mathf.PI * 0.5f);

                float rotationSpeed = 72.0f * Mathf.Min(1.0f, t);

                transform.localEulerAngles += new Vector3(0.0f, rotationSpeed * Time.fixedDeltaTime, 0.0f);
                // transform.localScale = scale * scalingFactor;
                transform.localPosition = pos + new Vector3(0.0f, heightFactor * height, 0.0f);

                if (timeCollectable > 3.0f)
                {
                    if (timeCollectable > 5.0f) Destroy(gameObject);

                    foreach (Renderer renderer in renderers)
                    {
                        renderer.enabled = (2.0f * timeCollectable - Mathf.Floor(2.0f * timeCollectable)) > 0.5f;
                    }
                }
                break;
        };


    }

    private void OnTriggerEnter(Collider collider)
    {
        PickUpHat(collider);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collider collider = collision.collider;
        PickUpHat(collider);
    }

    private void PickUpHat(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HatStack hs = other.gameObject.GetComponentInChildren<HatStack>();

            if (hs != null && (state == State.Collectable || state == State.Launched))
            {
                if (state == State.Launched)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }

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


                PlayerInput pi = other.gameObject.GetComponent<PlayerInput>();
                StatsManager.instance.HatPickedUp(pi.playerIndex);

                AudioManager.instance.Play("HatCollect3");
                hs.pushHat(gameObject);

                transform.localScale = scalingFactor * Vector3.one;

                hatOutline.SetFloat("_Outline_Thickness", 0.0025f);
            }
        }
    }
}
