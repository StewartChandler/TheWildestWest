using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;


[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private CharacterController controller;
    private HatStack hatStack;
    private Vector3 playerVelocity;
    private float playerSpeed = 13f;
    private float throwingSpeed = 30f;
    public float pickupRange = 3f;
    private ThrowableObject pickedObject;
    private float playerRadius;
    private float objMass;
    public float invincibilityOnHit = 1f;
    private float nextHit = 0.0f;
    private Vector3 respawnOffset = new Vector3(0, 35);

    private Vector2 movementInput = Vector2.zero;
    public GameObject playerPrefab;
    public GameManager gameManager;

    private float gravityValue = -9.81f;
    private float vibrationIntensity = 1f;
    private bool isVibrating = false;
    private float vibrationDuration = 0.2f; // Adjust the duration as needed
    private PlayerInput playerInput;
    private Color playerColor;
    private Material playerMaterial;

    private ThrowableObject prevHighlightedObject;

    private float timeTilActive = 0f;
    private State state = State.Active;

    private const float STUNTIME = 0.5f;
    private const float FROZENTIME = 2.0f;
    private const float PICKUPANIMLENGTH = 0.3f;

    private enum State
    {
        Active,
        HitStun,
        Frozen,
        PickupAnim,
    };

    public GameObject playerIndicatorTop;
    public GameObject playerIndicatorBottom;


    Scene currentScene;

    private GameObject hitEffect;

    private void makeActive()
    {
        state = State.Active;
        timeTilActive = 0.0f;
    }


    private void Start()
    {
        currentScene = SceneManager.GetActiveScene();
        Debug.Log(currentScene.name);
        controller = gameObject.GetComponent<CharacterController>();
        hatStack = gameObject.GetComponentInChildren<HatStack>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponentInChildren<Animator>();

        // get color of the player

        int playerIndex = gameManager.GetPlayerIndexFromInput(playerInput);

        if (playerIndex != -1)
        {
            playerColor = gameManager.playerColors[playerIndex];
            playerMaterial = gameManager.playerMaterials[playerIndex];
        }
        else
        {
            playerColor = new Color(0, 0, 0);
            playerMaterial = gameManager.playerMaterials[0];
        }

        // calculate distance away
        Vector3 psize = gameObject.GetComponent<Collider>().bounds.size;
        Vector2 psizexy = new Vector2(psize.x, psize.y);
        playerRadius = 0.5f * psizexy.magnitude;

        // think this is bad performance wise
        hitEffect = Resources.Load<GameObject>("vfx_graph_onhit");
        if (hitEffect == null)
        {
            Debug.Log("could not load hit effect");
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        currentScene = SceneManager.GetActiveScene();
        if (currentScene.name != "PlayerSelect" && (currentScene.name != "PlayerSelectMap" || context.control.name != "buttonSouth"))
        {
            if (context.performed && state == State.Active)
            {
                if (pickedObject == null)
                {
                    state = State.PickupAnim;
                    timeTilActive = PICKUPANIMLENGTH;

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
        if (currentScene.name != "PlayerSelect" && gameManager.endScreen == false && gameManager.firstPass == true)
        {
            switch (state)
            {
                case State.Active:
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
                        animator.SetBool("isMoving", true);
                    }
                    else
                    {
                        animator.SetBool("isMoving", false);
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
                    break;
                case State.HitStun:
                    timeTilActive -= Time.fixedDeltaTime;
                    if (timeTilActive < 0f)
                    {
                        makeActive();
                    }
                    break;
                case State.Frozen:
                    timeTilActive -= Time.fixedDeltaTime;
                    if (timeTilActive < 0f)
                    {
                        makeActive();
                    }
                    break;
                case State.PickupAnim:
                    // coresponds to 6 frames
                    const float PICKUPTIME = PICKUPANIMLENGTH - 0.1f;
                    // then active for 3 frames
                    const float PICKUPENDTIME = PICKUPTIME - 0.05f;
                    timeTilActive -= Time.fixedDeltaTime;
                    if (timeTilActive <= PICKUPTIME && timeTilActive > PICKUPENDTIME && pickedObject == null)
                    {
                        animator.SetBool("isPickup", true);
                        PickUpClosestObject();
                    }

                    if (timeTilActive < 0f)
                    {
                        animator.SetBool("isPickup", false);
                        makeActive();
                    }
                    break;
            }


            // Hihglight objecrs
            HighlightClosestObject();
        }
        else
        {
            Vector3 move = new Vector3(movementInput.x, 0, movementInput.y);
            animator.SetBool("isMoving", false);

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

            if (closestObj != null)
            {
                closestObj.pickupObject(transform, playerRadius);
                playPickUpSound(closestObj);
                pickedObject = closestObj;
            }
        }
    }

    void HighlightClosestObject()
    {
        ThrowableObject closestObj = ThrowableObject.getClosestAvailableObj(transform.position, hatStack.getNumHats(), pickupRange);

        if (closestObj != null)
        {
            closestObj.activateHighlight(playerColor);
        }


        if (prevHighlightedObject != null && prevHighlightedObject != closestObj)
        {
            prevHighlightedObject.removeHighlight();
        }
        prevHighlightedObject = closestObj;
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
                animator.SetBool("isThrowing", true);
                PlayerInput selfInput = GetComponent<PlayerInput>();
                StatsManager.instance.ItemThrown(selfInput.playerIndex);

                AudioManager.instance.Play("Throw1", "Throw2");
                pickedObject.throwObject(transform.forward, throwingSpeed);
                pickedObject = null;
                animator.SetBool("isThrowing", false);
            }
        }
    }

    public void takeDamage(Vector3 displ, bool frozen = false)
    {
        if (Time.time > nextHit)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity, null);
            transform.forward = displ; // turn to face where you got hit from
            nextHit = Time.time + invincibilityOnHit;
            AudioManager.instance.Play("Hit2");

            PlayerInput selfInput = GetComponent<PlayerInput>();
            StatsManager.instance.HatLost(selfInput.playerIndex);

            if (frozen)
            {
                state = State.Frozen;
                timeTilActive = FROZENTIME;
            }
            else
            {
                state = State.HitStun;
                timeTilActive = STUNTIME;
            }

            if (hatStack.getNumHats() > 0)
            {
                hatStack.popHat(displ);
                if (pickedObject != null)
                {
                    pickedObject.dropObject();
                    pickedObject = null;
                }
            }
            // This statement enables player death

            // else
            // {
            //    KillPlayer();
            // }
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

    public void DisableIndicator()
    {
        playerIndicatorTop.SetActive(false);
        playerIndicatorBottom.SetActive(false);
    }

    public void EnableIndicator()
    {
        playerIndicatorTop.SetActive(true);
        playerIndicatorBottom.SetActive(true);

        // get the int value of the later "ontop"
        int ontopLayer = LayerMask.NameToLayer("ontop");

        // apply the layer to the indicator
        playerIndicatorTop.layer = ontopLayer;
        playerIndicatorBottom.layer = ontopLayer;

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

        gameObject.tag = "Untagged";
        selfInput.DeactivateInput();
    }

    public void RespawnPlayer()
    {
        PlayerInput selfInput = GetComponent<PlayerInput>();
        CharacterController characterController = selfInput.GetComponent<CharacterController>();
        characterController.enabled = false;
        selfInput.transform.position = gameManager.playerSpawns[selfInput.playerIndex].position + respawnOffset;
        selfInput.transform.rotation = gameManager.playerSpawns[selfInput.playerIndex].rotation;
        characterController.enabled = true;
    }

    private void playPickUpSound(ThrowableObject closestObj)
    {
        // Debug.Log(closestObj.transform.parent.name);
        // if (closestObj.transform.parent.name == "Barrel" || closestObj.transform.parent.parent.name == "Barrel")
        // {
        //     AudioManager.instance.Play("BarrelPickUp1", "BarrelPickUp2");
        // }
        // else
        // {
        AudioManager.instance.Play("PickUp1");
        // }
    }
}