using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;




public class PlayerSelect : MonoBehaviour
{
    // Create 4 public spawn platform objects
    public GameObject[] spawnPlatforms = new GameObject[4];
    private int numPlayers = 0;
    float scale = 0.01f;
    private float scaleSpeed = 2f;
    private bool isScaling = false;
    private Vector3 initialScale = new Vector3(0f, 0f, 0f);
    private Vector3 targetScale = new Vector3(0.8f, 0.8f, 0.8f);
    private GameManager gameManager;
    private PlayerInput[] playerInputs = new PlayerInput[4];

    public Transform[] playerUIs = new Transform[4];
    public Transform[] playerCubes = new Transform[4];

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            gameManager.testStart = true;

        }
    }
    private void OnPlayerJoined(PlayerInput playerInput)
    {
        SetPlayerPositionAndColor(playerInput.transform);
        // if (!isScaling)
        // {
        //     StartCoroutine(ScaleObject(playerInput.transform));
        // }
        playerInputs[numPlayers - 1] = playerInput;
        playerInput.name = "Player" + numPlayers;
        gameManager.numPlayers = numPlayers;
        gameManager.players = playerInputs;
        playerInput.transform.parent = transform;

        // set the ui elements to active
        playerUIs[numPlayers - 1].gameObject.SetActive(true);

        // set the cube to the correct color
        playerCubes[numPlayers - 1].GetComponent<Renderer>().material.color = gameManager.playerColors[numPlayers - 1];
        gameManager.isPlayerAlive[numPlayers - 1] = true;
    }

    void SetPlayerPositionAndColor(Transform player)
    {
        CharacterController controller = player.GetComponent<CharacterController>();
        // Set the player's position to 4 above the spawn platform
        controller.enabled = false;
        player.position = spawnPlatforms[numPlayers].transform.position + new Vector3(0f, -3f, -2f);
        player.rotation = spawnPlatforms[numPlayers].transform.rotation * Quaternion.Euler(0f, 180f, 0f);
        controller.enabled = true;
        // player.localScale = new Vector3(1f, 1f, 1f);

        // Set the player's direction indicator color
        Renderer indicatorTop = player.Find("DirectionIndicatorTop").GetComponent<Renderer>();
        Renderer indicatorBottom = player.Find("DirectionIndicatorBottom").GetComponent<Renderer>();
        indicatorTop.material.color = gameManager.playerColors[numPlayers];
        indicatorBottom.material.color = gameManager.playerColors[numPlayers];

        // Set the players material color
        Transform innerPlayer = player.Find("Player");
        if (innerPlayer != null)
        {
            Transform childTransform = innerPlayer.transform.Find("PR_Basemodel_01_V2:Body_grp");
            if (childTransform != null)
            {
                GameObject innerInnerPlayer = childTransform.gameObject;

                foreach (Transform child in innerInnerPlayer.transform)
                {
                    SkinnedMeshRenderer childRenderer = child.GetComponent<SkinnedMeshRenderer>();

                    //if (!childRenderer) continue;
                    Debug.Log(gameManager.playerMaterials[numPlayers] == null);
                    childRenderer.material = gameManager.playerMaterials[numPlayers];
                }
            }
        }

        // Increment the number of players
        numPlayers++;
    }
    private IEnumerator ScaleObject(Transform player)
    {
        isScaling = true;
        while (scale < 1.0f)
        {
            scale += scaleSpeed * Time.deltaTime;
            player.localScale = Vector3.Lerp(initialScale, targetScale, scale);
            yield return null;
        }
        isScaling = false;
        scale = 0.01f;
    }

}
