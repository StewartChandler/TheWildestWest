using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;


public class EndRoundScreen : MonoBehaviour
{
    // Start is called before the first frame update
    private GameManager gameManager;
    private GameObject endRoundScreen;
    public Transform[] endRoundSpawnPoints;
    private GameObject playerManager;

    public TextMeshProUGUI[] scores;
    public TextMeshProUGUI message;
    private string[] messages = new string[]
        {
            "Yeeeeeehaw Player {0} Takes The Dub!",
            "Player {0} Is COOKING!",
            "POP OFF Player {0}!",
            "Holay Molay Player {0}!",
            "Player {0} Is Hitting The Griddy!",
            "NO CAP Player {0} Is Popping!",
            "Player {0} Is Hitting The Nae Nae",
            "Calm Down Player {0}!",
            "SHEESH Player {0} Takes The Round!",
        };
    void Start()
    {
        // get game manager
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.endRoundScreen = this;
        endRoundScreen = transform.Find("EndRoundScreen").gameObject;
        playerManager = GameObject.Find("PlayerManager");

    }
    public IEnumerator DisplayEndRoundUI()
    {
        UpdateScores();
        UpdateMessage();
        ShowUI();
        yield return new WaitForSeconds(4.0f);
        HideUI();

        gameManager.ending = true;
    }
    // Hide the game obbject
    private void HideUI()
    {
        // Set the child with the name EndRoundScreen to unactive
        endRoundScreen.SetActive(false);
    }
    private void ShowUI()
    {
        PlayerInput[] playerInputs = playerManager.GetComponentsInChildren<PlayerInput>();
        // activate playerinput and reset the scene
        foreach (PlayerInput playerInput in playerInputs)
        {
            // move each player to their respective spawn point
            CharacterController characterController = playerInput.GetComponent<CharacterController>();
            characterController.enabled = false;
            playerInput.transform.position = endRoundSpawnPoints[playerInput.playerIndex].position;
            characterController.enabled = true;
            ChangeLayerRecursively(playerInput.transform, "UI");
            playerInput.ActivateInput();
        }
        endRoundScreen.SetActive(true);
    }
    void ChangeLayerRecursively(Transform currentTransform, string layerName)
    {
        // Find the layer by name
        int targetLayer = LayerMask.NameToLayer(layerName);

        // Check if the targetLayer exists
        if (targetLayer != -1)
        {
            // Change the layer of the current GameObject
            currentTransform.gameObject.layer = targetLayer;

            // Iterate through the children and recursively change their layers
            foreach (Transform child in currentTransform)
            {
                ChangeLayerRecursively(child, layerName);
            }
        }
        else
        {
            Debug.LogWarning("Layer " + layerName + " not found.");
        }
    }
    void UpdateScores()
    {
        for (int i = 0; i < scores.Length; i++)
        {
            if (gameManager.currWinner == i)
            {
                scores[i].color = Color.green;
            }
            else
            {
                scores[i].color = Color.white;
            }
            scores[i].text = gameManager.playerRoundHats[i].ToString();
        }
    }
    void UpdateMessage()
    {
        int randomIndex = Random.Range(0, messages.Length);
        string winnerText = messages[randomIndex];
        winnerText = string.Format(winnerText, gameManager.currWinner + 1);
        message.text = winnerText;
    }
}
