using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Linq;
using UnityEngine.EventSystems;


public class EndGameUI : MonoBehaviour
{
    // Start is called before the first frame update
    private GameManager gameManager;
    private int currScoreCount = 0;
    private float initialScale = 0.75f;
    private float scaleFactor = 0.25f;
    public Transform[] EndGameSpawnPoints;
    private GameObject playerManager;

    public TextMeshProUGUI[] scores;
    public Transform scoreParent;
    public Transform stats;
    public TextMeshProUGUI[] PlayerNums = new TextMeshProUGUI[4];
    public Transform PlayerStats;
    public TextMeshProUGUI[] FinalScores = new TextMeshProUGUI[4];
    public Transform FinalScoreStats;
    public TextMeshProUGUI[] Thrown = new TextMeshProUGUI[4];
    public Transform ThrownStats;
    public TextMeshProUGUI[] HatsGained = new TextMeshProUGUI[4];
    public Transform HatsGainedStats;
    public TextMeshProUGUI[] HatsLost = new TextMeshProUGUI[4];
    public Transform HatsLostStats;
    public TextMeshProUGUI[] Falls = new TextMeshProUGUI[4];
    public Transform FallsStats;
    public Transform MainMenuButton;
    public GameObject SelectedButton;
    public TextMeshProUGUI message;
    private string[] messages = new string[]
        {
            "Winner!",
            "2nd Place!",
            "3rd Place",
            "4th Place."
        };

    private int winnerIndex = 0;
    private int[] positions = new int[4];
    PlayerInput[] playerInputs;
    private Color gold = new Color(255f, 215f, 0f);

    void Start()
    {
        Debug.Log("ENDGAMEUI START");
        // get game manager
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerManager = GameObject.Find("PlayerManager");
        playerInputs = playerManager.GetComponentsInChildren<PlayerInput>();
        findWinner();

        GeneratePositions();


        // Start the show
        StartCoroutine(DisplayEndGameUI());
    }


    /* The order is going to be 
        1. Display all the characters
        2. Display the scores
        3. Display the messages
        4. Display the stats
    */
    public IEnumerator DisplayEndGameUI()
    {
        // // Phase 1: Display characters and Game Over message
        // // Wait 3 seconds
        // DisplayCharacters();
        // UpdateMessage("Game Over. Counting Scores...", Color.white);
        // yield return new WaitForSeconds(3f);

        // // Phase 2: Count the scores and make the characters grow
        // // Wait 1.2 seconds in between each count
        // while (currScoreCount < gameManager.maxScore)
        // {
        //     CountScore();
        //     yield return new WaitForSeconds(1.2f);
        // }

        // // Phase 3: Display the winner
        // // Wait 3 seconds
        // UpdateMessage("Player " + (winnerIndex + 1) + " Wins!", gold);
        // placeMessages();
        // yield return new WaitForSeconds(3f);

        // Phase 5: Remove curr UI
        deletePlayerManager();
        disableScores();

        // Phase 6: Display the stats
        UpdateMessage("Displaying Stats...", Color.white);
        calculateScores();
        enableStats();
        yield return new WaitForSeconds(1f);
        displayPlayers();
        yield return new WaitForSeconds(1f);
        displayFinalScores();
        yield return new WaitForSeconds(1f);
        displayThrown();
        yield return new WaitForSeconds(1f);
        displayHatsGained();
        yield return new WaitForSeconds(1f);
        displayHatsLost();
        yield return new WaitForSeconds(1f);
        displayFalls();
        yield return new WaitForSeconds(1f);
        displayMainMenu();

    }

    private void DisplayCharacters()
    {
        Debug.Log("DisplayCharacters");
        message.text = "";
        for (int i = 0; i < playerInputs.Length; i++)
        {
            CharacterController controller = playerInputs[i].GetComponent<CharacterController>();
            controller.enabled = false;
            playerInputs[i].transform.position = EndGameSpawnPoints[i].position;
            playerInputs[i].transform.localScale = new Vector3(initialScale, initialScale, initialScale);
            controller.enabled = true;
        }
    }
    private void UpdateMessage(string message, Color color)
    {
        this.message.text = message;
        this.message.color = color;
    }

    private void CountScore()
    {
        Debug.Log("CountScore");
        currScoreCount++;
        for (int i = 0; i < gameManager.playerScores.Length; i++)
        {
            if (gameManager.playerScores[i] >= currScoreCount)
            {
                playerInputs[i].transform.localScale = new Vector3(initialScale + scaleFactor * currScoreCount, initialScale + scaleFactor * currScoreCount, initialScale + scaleFactor * currScoreCount);
                scores[i].text = currScoreCount.ToString();
            }
        }
    }

    void placeMessages()
    {
        for (int i = 0; i < gameManager.playerScores.Length; i++)
        {
            if (i >= gameManager.numPlayers)
            {
                scores[i].text = "";
            }
            else
            {
                if (positions[i] == 1)
                {
                    scores[i].fontSize = 0.8f;
                    scores[i].color = gold;
                    scores[i].text = messages[0];
                }
                else if (positions[i] == 2)
                {
                    scores[i].fontSize = 0.8f;
                    scores[i].text = messages[1];
                }
                else if (positions[i] == 3)
                {
                    scores[i].fontSize = 0.8f;
                    scores[i].text = messages[2];
                }
                else if (positions[i] == 4)
                {
                    scores[i].fontSize = 0.8f;
                    scores[i].text = messages[3];
                }
            }
        }
    }

    // Helper to generate the positions of the players
    // Ie if player 4 > Player 2 > Player 3 > Player 1, then the positions are 4, 2, 3, 1
    void GeneratePositions()
    {
        // if the score is maxScore, they are in first place
        positions[winnerIndex] = 1;

        bool hasTwoPoints = false;
        // if you have maxScore - 1 points, you are guaranteed to be in second place
        for (int i = 0; i < gameManager.playerScores.Length; i++)
        {
            if (gameManager.playerScores[i] == gameManager.maxScore - 1)
            {
                hasTwoPoints = true;
                positions[i] = 2;
            }
        }

        // if someone has 2 points, you are third or worse if you have maxScore - 2 points
        for (int i = 0; i < gameManager.playerScores.Length; i++)
        {
            if (gameManager.playerScores[i] == gameManager.maxScore - 2)
            {
                if (hasTwoPoints == false)
                {
                    positions[i] = 2;
                }
                else
                {
                    positions[i] = 3;
                }
            }
        }
        int maxPos = positions.Max();
        for (int j = 0; j < gameManager.playerScores.Length; j++)
        {
            if (positions[j] == 0)
            {
                positions[j] = maxPos + 1;
            }
        }
        // debug each position from 1 to 4
        for (int j = 0; j < gameManager.playerScores.Length; j++)
        {
            Debug.Log("Player " + (j + 1) + " is in position " + positions[j]);
        }



    }
    void findWinner()
    {

        // get the winner
        for (int i = 0; i < gameManager.playerScores.Length; i++)
        {
            if (gameManager.playerScores[i] >= gameManager.maxScore)
            {
                winnerIndex = i;
            }
        }
    }
    void deletePlayerManager()
    {
        Destroy(playerManager);
    }
    void disableScores()
    {
        scoreParent.gameObject.SetActive(false);
    }
    void enableStats()
    {
        stats.gameObject.SetActive(true);
    }

    void calculateScores()
    {
        for (int i = 0; i < 4; i++)
        {
            if (i < gameManager.numPlayers)
            {
                PlayerNums[i].text = "P" + (i + 1);
                printMinMax(FinalScores, gameManager.playerScores);
                printMinMax(Thrown, StatsManager.instance.itemsThrown);
                printMinMax(HatsGained, StatsManager.instance.hatsPickedUp);
                printMinMax(HatsLost, StatsManager.instance.hatsLost, true);
                printMinMax(Falls, StatsManager.instance.timesFallen, true);

            }
            else
            {
                PlayerNums[i].text = "";
                FinalScores[i].text = "";
                Thrown[i].text = "";
                HatsGained[i].text = "";
                HatsLost[i].text = "";
                Falls[i].text = "";
            }
        }
    }
    void printMinMax(TextMeshProUGUI[] textArr, int[] scoreArr, bool rev = false)
    {
        int min = scoreArr.Min();
        int max = scoreArr.Max();
        for (int i = 0; i < 4; i++)
        {
            textArr[i].text = scoreArr[i].ToString();
            if (scoreArr[i] == min)
            {
                if (rev)
                {
                    textArr[i].color = Color.green;
                }
                else
                {
                    textArr[i].color = Color.red;
                }
            }
            else if (scoreArr[i] == max)
            {
                if (rev)
                {
                    textArr[i].color = Color.red;
                }
                else
                {
                    textArr[i].color = Color.green;
                }
            }
            else
            {
                textArr[i].color = Color.white;
            }
        }
    }
    void displayPlayers()
    {
        PlayerStats.gameObject.SetActive(true);
    }
    void displayFinalScores()
    {
        FinalScoreStats.gameObject.SetActive(true);
    }
    void displayThrown()
    {
        ThrownStats.gameObject.SetActive(true);
    }
    void displayHatsGained()
    {
        HatsGainedStats.gameObject.SetActive(true);
    }
    void displayHatsLost()
    {
        HatsLostStats.gameObject.SetActive(true);
    }
    void displayFalls()
    {
        FallsStats.gameObject.SetActive(true);
    }
    void displayMainMenu()
    {
        MainMenuButton.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(SelectedButton);

    }

}