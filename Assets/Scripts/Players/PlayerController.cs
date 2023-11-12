using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;


[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{

    private CharacterController controller;
    private HatStack hatStack;
    private Vector3 playerVelocity;
    private float playerSpeed = 13f;
    private float throwingSpeed = 30f;
    public float pickupRange = 5f;
    private ThrowableObject pickedObject;
    private float playerRadius;
    private float objMass;
    public float invincibilityOnHit = 1f;
    private float nextHit = 0.0f;

    private Vector2 movementInput = Vector2.zero;
    public GameObject playerPrefab;
    public GameManager gameManager;

    private float gravityValue = -9.81f;
    private float vibrationIntensity = 1f;
    private bool isVibrating = false;
    private float vibrationDuration = 0.2f; // Adjust the duration as needed
    private PlayerInput playerInput;

    private ThrowableObject prevHighlightedObject;


    Scene currentScene;

    private GameObject hitEffect;


    private void Start()
    {
        currentScene = SceneManager.GetActiveScene();
        Debug.Log(currentScene.name);
        controller = gameObject.GetComponent<CharacterController>();
        hatStack = gameObject.GetComponentInChildren<HatStack>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerInput = GetComponent<PlayerInput>();


        // calculate distance away
        Vector3 psize = gameObject.GetComponent<Collider>().bounds.size;
        Vector2 psizexy = new Vector2(psize.x, psize.y);
        playerRadius = 0.5f * psizexy.magnitude;

        // think this is bad performance wise
        hitEffect = Resources.Load<GameObject>("vfx_graph_onhit");
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        currentScene = SceneManager.GetActiveScene();
        if (currentScene.name != "PlayerSelect")
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
    }

    void FixedUpdate()
    {
        if (currentScene.name != "PlayerSelect" && gameManager.firstPass == true)
        {
            Vector3 move = new Vector3(movementInput.x, 0, movementInput.y);
            float speedMul = 1.0f;
            if (pickedObject != null)
            {
                speedMul = Mathf.Pow(1.25f, 1.0f - Mathf.Max(objMass, 1.0f));
            }
            controller.Move(move * Time.fixedDeltaTime * playerSpeed * speedMul);

            if (move != Vector3.zero)
            {
                gameObject.transform.forward = move;
            }
            if (controller.isGrounded)
            {
                // Reset the vertical velocity to 0 when grounded
                playerVelocity.y = 0;
            }
            else
            {
                playerVelocity.y += gravityValue * Time.fixedDeltaTime;
                controller.Move(playerVelocity * Time.fixedDeltaTime);
            }


            // Hihglight objecrs
            ThrowableObject closestObj = ThrowableObject.getClosestAvailableObj(transform.position, hatStack.getNumHats(), pickupRange);
            closestObj.activateHighlight();

            if (prevHighlightedObject != null && prevHighlightedObject != closestObj)
            {
                prevHighlightedObject.removeHighlight();
            }
            prevHighlightedObject = closestObj;
        }
        else
        {
            Vector3 move = new Vector3(movementInput.x, 0, movementInput.y);

            if (move != Vector3.zero)
            {
                // Calculate the rotation angle based on the input.
                float targetAngleX = Mathf.Atan(move.x) * Mathf.Rad2Deg;
                float targetAngleZ = Mathf.Atan(move.z) * Mathf.Rad2Deg;

                // Calculate the angular velocity
                Vector3 angularVelocity = new Vector3(targetAngleZ, targetAngleX, 0f);

                // Set the transform's rotational velocity
                transform.Rotate(angularVelocity * Time.deltaTime * 10);
            }

        }
    }

    void PickUpClosestObject()
    {
        currentScene = SceneManager.GetActiveScene();
        if (currentScene.name != "PlayerSelect")
        {

            ThrowableObject closestObj = ThrowableObject.getClosestAvailableObj(transform.position, hatStack.getNumHats(), pickupRange);
            // Debug.Log(closestObj);

            if (closestObj != null)
            {
                closestObj.pickupObject(transform, playerRadius);
                playPickUpSound(closestObj);
                pickedObject = closestObj;
            }
        }
    }

    public void DropObject()
    {
        if (pickedObject != null)
        {
            pickedObject = null;
        }
    }

    void ThrowObject()
    {
        currentScene = SceneManager.GetActiveScene();

        if (currentScene.name != "PlayerSelect")
        {
            if (pickedObject != null)
            {
                AudioManager.instance.Play("Throw1", "Throw2");
                pickedObject.throwObject(transform.forward, throwingSpeed);
                pickedObject = null;
            }
        }
    }

    public void takeDamage(Vector3 displ)
    {
        if (Time.time > nextHit)
        {
            Instantiate(hitEffect);
            nextHit = Time.time + invincibilityOnHit;
            AudioManager.instance.Play("Hit1");

            if (hatStack.getNumHats() > 1)
            {
                hatStack.popHat(displ);
                if (pickedObject != null)
                {
                    pickedObject.dropObject();
                    pickedObject = null;
                }
            }
            else
            {
                KillPlayer();
            }
            StartVibration(vibrationIntensity, vibrationDuration);
        }
    }
    private void StartVibration(float intensity, float duration)
    {
        if (playerInput != null)
        {
            Gamepad gamepad = playerInput.devices[0] as Gamepad;
            if (gamepad != null)
            {
                gamepad.SetMotorSpeeds(intensity, intensity);
                isVibrating = true;
                StartCoroutine(StopVibrationAfterDuration(duration));
            }
        }
    }

    private IEnumerator StopVibrationAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);

        // Stop the vibration
        if (playerInput != null)
        {
            Gamepad gamepad = playerInput.devices[0] as Gamepad;
            if (gamepad != null)
            {
                gamepad.SetMotorSpeeds(0, 0);
                isVibrating = false;
            }
        }
    }

    public void KillPlayer()
    {
        if (hatStack.getNumHats() > 0)
        {
            hatStack.popAllHats();
        }
        Debug.Log("Player Died");
        PlayerInput selfInput = GetComponent<PlayerInput>();
        if (selfInput != null)
        {
            gameManager.isPlayerAlive[selfInput.playerIndex] = false;
        }

        selfInput.DeactivateInput();
    }

    private void playPickUpSound(ThrowableObject closestObj)
    {
        // Debug.Log(closestObj.transform.parent.name);
        if (closestObj.transform.parent.name == "Barrel" || closestObj.transform.parent.parent.name == "Barrel")
        {
            AudioManager.instance.Play("BarrelPickUp1", "BarrelPickUp2");
        }
        else
        {
            AudioManager.instance.Play("PickUp1");
        }
    }
}