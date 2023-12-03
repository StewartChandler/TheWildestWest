using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Linq;
using UnityEngine.EventSystems;


public class EndGameUIDeath : MonoBehaviour
{
    // Start is called before the first frame update
    private GameManager gameManager;
    private int currScoreCount = 0;
    private float initialScale = 0.5f;
    private float scaleFactor = 0.0002f;
    public Transform[] EndGameSpawnPoints;
    private GameObject playerManager;
    private float scaleDuration = 1f;

    public TextMeshProUGUI[] scores;
    public Transform scoreParent;
    public TextMeshProUGUI[] scoresAdder;
    public Transform scoreAdderParent;
    public TextMeshProUGUI[] statsCurr;
    public Transform statsCurrParent;
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
    public GameObject StatsButton;
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
    private int[][] statArrays = new int[4][];
    private string[] statNames = new string[]
    {
        "Items Thrown",
        "Times Hit",
        "Hats Picked Up",
        "Times Fallen"
    };

    void Start()
    {
        Debug.Log("ENDGAMEUI START SCORE");
        // get game manager
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerManager = GameObject.Find("PlayerManager");
        playerInputs = playerManager.GetComponentsInChildren<PlayerInput>();

        // Assign the stat arrays
        statArrays[0] = StatsManager.instance.itemsThrown;
        statArrays[1] = StatsManager.instance.hatsLost;
        statArrays[2] = StatsManager.instance.hatsPickedUp;
        statArrays[3] = StatsManager.instance.timesFallen;

        // Music
        AudioManager.instance.Stop("Music1");
        AudioManager.instance.Play("Music3");

        // Start the show
        StartCoroutine(DisplayEndGameUIScore());

    }

    /* The order is going to be 
        1. Display all the characters
            a. include a 0 score
            b. include space for a + score
            c. include a place to keep count of stats
        2. Display hats collected and add to score
        3. Display the stats and add 400 for most and 200 for second most
        4. Display the winner
        5. button for stats
        6. button for main menu
    */
    public IEnumerator DisplayEndGameUIScore()
    {
        // Phase 1: Display all the characters
        UpdateMessage("Game Over. Calculating Score!", Color.white);
        DisplayCharacters();
        yield return new WaitForSeconds(1.5f);

        // Phase 2: Display hats collected and add to score
        UpdateMessage("Calculating Total Hats!", Color.white);
        countHats();
        printScores();
        resizeCharacters();
        yield return new WaitForSeconds(5f);

        // Phase 3: Display the stats and add 400 for most and 200 for second most
        for (int i = 0; i < 4; i++)
        {
            UpdateMessage("Calculating " + statNames[i] + "!", Color.white);
            if (i == 1 || i == 3)
            {
                countStat(statArrays[i], true);
            }
            else
            {
                countStat(statArrays[i]);
            }
            printScores();
            resizeCharacters();
            yield return new WaitForSeconds(5f);
        }

        // Phase 4: Display the winner
        calcuateWinner();
        UpdateMessage("Player " + (winnerIndex + 1) + " Wins!", gold);
        updateMessages();

        displayStatsButton();

    }

    private void DisplayCharacters()
    {
        Debug.Log("DisplayCharacters");
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

    private void countHats()
    {
        for (int i = 0; i < gameManager.playerScores.Length; i++)
        {
            if (i >= gameManager.numPlayers)
            {
                scoresAdder[i].text = "";
                statsCurr[i].text = "";
            }
            else
            {
                statsCurr[i].text = gameManager.playerHats[i].ToString();
                scoresAdder[i].text = "+" + (gameManager.playerHats[i] * 100).ToString();
                gameManager.playerScores[i] += gameManager.playerHats[i] * 100;
            }
        }
    }

    /**
        Helper that will take in an array of ints.
        For players that don't exist it will set the index to -1
        For players that do exist it will set the index to:
            1st will get 400 points if 3/4 players, 200 points if 2 players
            2nd will get 200 points if 3/4 players, 0 points if 2 players
        if there is a tie for 1st then all get 400 points
        if there is a tie for 2nd then all get 200 points
    */
    private void countStat(int[] statsArray, bool reverse = false)
    {

        // ie in the case of times fallen off and hats lost, less is better
        int[] temp = new int[4];
        if (reverse == true)
        {
            // assign stats array to a new array that is negative
            for (int i = 0; i < 4; i++)
            {

                temp[i] = -statsArray[i];

            }
        }
        else
        {
            temp = statsArray;
        }


        // calculate the position of each player
        int[] position = calculatePosition(temp);

        // convert the position to points
        int[] points = convertToPoint(position);

        // add the points to the player scores
        for (int i = 0; i < gameManager.playerScores.Length; i++)
        {
            if (i >= gameManager.numPlayers)
            {
                statsCurr[i].text = "";
            }
            else
            {
                statsCurr[i].text = statsArray[i].ToString();
                gameManager.playerScores[i] += points[i];
            }
        }

        // set the adder score
        for (int i = 0; i < gameManager.playerScores.Length; i++)
        {
            if (i >= gameManager.numPlayers)
            {
                scoresAdder[i].text = "";
            }
            else
            {
                scoresAdder[i].text = "+" + points[i].ToString();
            }
        }
    }

    // given an array of numbers, return an array of the same size with the positions of each number
    // if there is a tie then the position will be the same, and the next position is skipped
    private int[] calculatePosition(int[] scores)
    {
        int[] positions = new int[4];
        int currPosition = 1;
        for (int i = 0; i < 4; i++)
        {
            if (i >= gameManager.numPlayers)
            {
                positions[i] = 0;
            }
            else
            {
                int currScore = scores[i];
                for (int j = 0; j < 4; j++)
                {
                    if (currScore < scores[j])
                    {
                        currPosition++;
                    }
                }
                positions[i] = currPosition;
                currPosition = 1;
            }
        }
        return positions;
    }

    private int[] convertToPoint(int[] positions)
    {
        // given an array of positions, convert to points
        // if 1, then give firstPoints
        // if 2, then give secondPoints
        // if 3 or 4, then give 0 points
        // if player doesn't exist, then give -1 points
        int firstPlaceScore;
        int secondPlaceScore;
        if (gameManager.numPlayers == 2)
        {
            firstPlaceScore = 200;
            secondPlaceScore = 0;
        }
        else
        {
            firstPlaceScore = 400;
            secondPlaceScore = 200;
        }
        int[] points = new int[4];
        for (int i = 0; i < 4; i++)
        {
            if (i >= gameManager.numPlayers)
            {
                points[i] = -1;
            }
            else
            {
                if (positions[i] == 1)
                {
                    points[i] = firstPlaceScore;
                }
                else if (positions[i] == 2)
                {
                    points[i] = secondPlaceScore;
                }
                else
                {
                    points[i] = 0;
                }
            }
        }
        return points;
    }

    private void calcuateWinner()
    {
        int max = gameManager.playerScores.Max();
        for (int i = 0; i < gameManager.playerScores.Length; i++)
        {
            if (gameManager.playerScores[i] == max)
            {
                winnerIndex = i;
            }
        }
    }

    private void printScores()
    {
        for (int i = 0; i < gameManager.playerScores.Length; i++)
        {
            if (i >= gameManager.numPlayers)
            {
                scores[i].text = "";
            }
            else
            {
                scores[i].text = gameManager.playerScores[i].ToString();
            }
        }
    }

    private void resizeCharacters()
    {
        for (int i = 0; i < gameManager.numPlayers; i++)
        {
            StartCoroutine(ScaleCharacter(playerInputs[i].transform, initialScale + scaleFactor * gameManager.playerScores[i]));
        }
    }

    private IEnumerator ScaleCharacter(Transform characterTransform, float targetScale)
    {
        Vector3 initialScale = characterTransform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < scaleDuration)
        {
            characterTransform.localScale = Vector3.Lerp(initialScale, new Vector3(targetScale, targetScale, targetScale), elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure that the final scale is set correctly to avoid any small deviations
        characterTransform.localScale = new Vector3(targetScale, targetScale, targetScale);
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

    private void updateMessages()
    {
        // calculate the position of each player
        int[] playerPositions = calculatePosition(gameManager.playerScores);

        // change the score to have the respective messages
        for (int i = 0; i < gameManager.playerScores.Length; i++)
        {
            if (i >= gameManager.numPlayers)
            {
                statsCurr[i].text = "";
            }
            else
            {
                // make font 0.75
                statsCurr[i].fontSize = 0.75f;
                statsCurr[i].text = messages[playerPositions[i] - 1];
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
        EventSystem.current.SetSelectedGameObject(MainMenuButton.gameObject);

    }

    void displayStatsButton()
    {
        // make scoreadder disabled
        scoreAdderParent.gameObject.SetActive(false);

        StatsButton.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(StatsButton.gameObject);
    }

    public void statsButtonPressed()
    {
        StartCoroutine(displayStatsCoroutine());
    }

    public IEnumerator displayStatsCoroutine()
    {
        // Phase 5: Remove curr UI
        deletePlayerManager();
        disableScores();

        // disable the curr buttons
        StatsButton.gameObject.SetActive(false);

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

}