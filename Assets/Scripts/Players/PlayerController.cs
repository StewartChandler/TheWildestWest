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
    private ThrowableObject pickedObject;
    private float playerRadius;
    private float objMass;

    private Vector2 movementInput = Vector2.zero;
    public GameObject playerPrefab;


    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        hatStack = gameObject.GetComponentInChildren<HatStack>();

        // calculate distance away
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
    }

    void PickUpClosestObject()
    {
        ThrowableObject closestObj = ThrowableObject.getClosestAvailableObj(transform.position, hatStack.getNumHats(), pickupRange);
        // Debug.Log(closestObj);


        if (closestObj != null)
        {
            closestObj.pickupObject(transform, playerRadius);
            pickedObject = closestObj;
        }
    }
    void ThrowObject()
    {
        if (pickedObject != null)
        {
            pickedObject.throwObject(transform.forward, throwingSpeed);
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
                pickedObject.dropObject();
                pickedObject = null;
            }
        }
        else
        {
            // TODO: u ded
        }
    }
}