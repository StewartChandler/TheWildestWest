using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartCountdown : MonoBehaviour
{
    private float timeRemaining = 4;
    private bool timerIsRunning = false;
    public TextMeshProUGUI timeText;

    private bool gamePaused = false;
    public GameObject inGameTimer;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Pause the game
        Time.timeScale = 0;
        timeRemaining = 5;

        // Starts the timer automatically
        timerIsRunning = true;
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 2)
            {
                timeText.text = string.Format("ROUND {0}/3", gameManager.currRound);
                timeRemaining -= Time.unscaledDeltaTime; // Use unscaledDeltaTime to ignore time scale
            }
            else if (timeRemaining > 1)
            {
                timeRemaining -= Time.unscaledDeltaTime; // Use unscaledDeltaTime to ignore time scale
                timeText.text = "READY!";
            }
            else if (timeRemaining > 0)
            {
                timeText.text = "FIGHT!";
                timeRemaining -= Time.unscaledDeltaTime;
            }
            else
            {
                Debug.Log("Begin!");
                timeRemaining = 0;
                timerIsRunning = false;
                // Resume the game
                Time.timeScale = 1;
                timeText.text = "";
                timeText.gameObject.SetActive(false);
                inGameTimer.gameObject.SetActive(true);
            }
        }
    }
}
