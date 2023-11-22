using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartCountdown : MonoBehaviour
{
    private float timeRemaining = 2;
    public bool timerIsRunning = false;
    public TextMeshProUGUI timeText;

    private bool gamePaused = false;
    public TextMeshProUGUI inGameTimer;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Pause the game
        Time.timeScale = 0;
        if (gameManager.firstGame == true)
        {
            timeRemaining = 4;
        }

        // Starts the timer automatically
        timerIsRunning = true;
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 2)
            {
                timeRemaining -= Time.unscaledDeltaTime; // Use unscaledDeltaTime to ignore time scale
                timeText.text = string.Format("First to {0} points wins!", gameManager.maxScore);
            }
            if (timeRemaining > 1)
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
