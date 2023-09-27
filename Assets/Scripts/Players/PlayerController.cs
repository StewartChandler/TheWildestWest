using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private HatStack hatStack;
    private Vector3 playerVelocity;
    private float playerSpeed = 6.5f;
    private float throwingSpeed = 30f;
    private float gravityValue = -9.81f;
    public float pickupRange = 5f;
    private Transform pickedObject;
    private Rigidbody pickedRigidbody;
    private Vector3 pickUpOffset = new Vector3(1.0f, 2f, 0.0f);

    private Vector2 movementInput = Vector2.zero;
    public GameObject playerPrefab;


    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        hatStack = gameObject.GetComponentInChildren<HatStack>();
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
        controller.Move(move * Time.fixedDeltaTime * playerSpeed);

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
            Vector3 desiredPosition = transform.position + transform.forward * pickUpOffset.z + transform.right * pickUpOffset.x + transform.up * pickUpOffset.y;

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
                    closestDistance = distance;
                    closestCollider = collider;
                }
            }
        }

        if (closestCollider != null)
        {
            pickedObject = closestCollider.transform;
            pickedRigidbody = pickedObject.GetComponent<Rigidbody>();
            pickedRigidbody.isKinematic = true;

            // Move the object closer to the player.
            pickedObject.position = transform.position + pickUpOffset;
        }
    }
    void ThrowObject()
    {
        if (pickedObject != null)
        {
            pickedRigidbody.isKinematic = false;
            pickedRigidbody.velocity = (transform.forward * throwingSpeed); // Adjust the throw force as needed.
            pickedObject = null;
        }
    }

    public void takeDamage()
    {
        if (hatStack.getNumHats() > 0)
        {
            hatStack.popHat();
        }
        else
        {
            // TODO: u ded
        }
    }
}