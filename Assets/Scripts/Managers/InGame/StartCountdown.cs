using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartCountdown : MonoBehaviour
{
    public float timeRemaining = 5;
    public bool timerIsRunning = false;
    public TextMeshProUGUI timeText;

    private bool gamePaused = false;
    public TextMeshProUGUI inGameTimer;

    private void Start()
    {
        // Pause the game
        Time.timeScale = 0;

        // Starts the timer automatically
        timerIsRunning = true;
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 1)
            {
                timeRemaining -= Time.unscaledDeltaTime; // Use unscaledDeltaTime to ignore time scale
                float seconds = Mathf.FloorToInt(timeRemaining);
                timeText.text = seconds.ToString();
            }
            else if (timeRemaining > 0)
            {
                timeText.text = "GO!";
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
