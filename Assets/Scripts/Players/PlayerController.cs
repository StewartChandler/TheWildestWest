using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private HatStack hatStack;
    private Vector3 playerVelocity;
    private float playerSpeed = 13f;
    private float throwingSpeed = 30f;
    private float gravityValue = -9.81f;
    public float pickupRange = 5f;
    private Transform pickedObject;
    private Rigidbody pickedRigidbody;
    private Vector3 pickUpOffset = Vector3.Normalize(new Vector3(1.0f, 2f, 0.0f));
    private float distAway;
    private float playerRadius;
    private float objMass;

    private Vector2 movementInput = Vector2.zero;
    public GameObject playerPrefab;


    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        hatStack = gameObject.GetComponentInChildren<HatStack>();
        Vector3 psize = gameObject.GetComponent<Collider>().bounds.size;
        Vector2 psizexy = new Vector2(psize.x, psize.y);
        playerRadius = 0.5f * psizexy.magnitude;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        if (context.performed)
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
    }

    void FixedUpdate()
    {
        Vector3 move = new Vector3(movementInput.x, 0, movementInput.y);
        float speedMul = 1.0f;
        if (pickedObject != null) {
            speedMul = Mathf.Pow(1.25f, 1.0f - Mathf.Max(objMass, 1.0f));
        }
        controller.Move(move * Time.fixedDeltaTime * playerSpeed * speedMul);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        playerVelocity.y += gravityValue * Time.fixedDeltaTime;
        controller.Move(playerVelocity * Time.fixedDeltaTime);

        // Moving the object around with the player if it's picked up
        if (pickedObject != null)
        {
            // Calculate the desired position based on player's position and forward direction.
            Vector3 desiredPosition = transform.position + (playerRadius + distAway) * (transform.forward * pickUpOffset.z + transform.right * pickUpOffset.x) + transform.up * pickUpOffset.y;

            // Lerp the object's position to the desired position for smooth movement.
            pickedObject.position = Vector3.Lerp(pickedObject.position, desiredPosition, Time.fixedDeltaTime * 10f);

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
                    float colMass = collider.GetComponent<Rigidbody>().mass;
                    if (colMass <= hatStack.getNumHats() + 1)
                    {
                        closestDistance = distance;
                        closestCollider = collider;
                        objMass = colMass;
                    }
                }
            }
        }

        if (closestCollider != null)
        {
            pickedObject = closestCollider.transform;
            pickedRigidbody = pickedObject.GetComponent<Rigidbody>();
            pickedRigidbody.isKinematic = false;
            pickedRigidbody.mass = 0;
            pickedRigidbody.useGravity = false;

            // makes the object be futher away for larger objects
            distAway = 0;
            foreach (Collider collider in closestCollider.gameObject.GetComponents<Collider>()) { 
                distAway = Mathf.Max(distAway, Vector3.Magnitude(collider.bounds.size));
            }

            // Move the object closer to the player.
            pickedObject.position = transform.position + (playerRadius + distAway) * pickUpOffset;
        }
    }
    void ThrowObject()
    {
        if (pickedObject != null)
        {
            pickedRigidbody.isKinematic = false;
            pickedRigidbody.mass = objMass;
            pickedRigidbody.useGravity = true;
            pickedRigidbody.velocity = (transform.forward * throwingSpeed); // Adjust the throw force as needed.
            pickedObject = null;
        }
    }

    public void takeDamage()
    {
        if (hatStack.getNumHats() > 0)
        {
            hatStack.popHat();
            if (pickedObject != null)
            {
                pickedRigidbody.isKinematic = false;
                pickedObject = null;
            }
        }
        else
        {
            // TODO: u ded
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject collisionObject = collision.gameObject;
        Rigidbody collisionRigidbody = collisionObject.GetComponent<Rigidbody>();
        if (collision.gameObject.tag == "Throwable" && collisionRigidbody.velocity.magnitude >= throwingSpeed / 3)
        {
            takeDamage();
        }
    }
}