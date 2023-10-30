using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public float timeRemaining = 4;
    public bool timerIsRunning = false;
    public TextMeshProUGUI timeText;

    private bool gamePaused = false;
    private GameManager gameManager;


    private void Start()
    {
        // Starts the timer automatically
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        timerIsRunning = false;
    }

    void Update()
    {
        if (gameManager.numPlayers >= 2 && gameManager.numPlayers == gameManager.playersReady)
        {
            timerIsRunning = true;
            timeText.gameObject.SetActive(true);

        }
        else
        {
            timerIsRunning = false;
            timeText.gameObject.SetActive(false);
            timeRemaining = 4;
        }
        if (timerIsRunning)
        {
            if (timeRemaining > 1.1)
            {
                timeRemaining -= Time.unscaledDeltaTime; // Use unscaledDeltaTime to ignore time scale
                float seconds = Mathf.FloorToInt(timeRemaining);
                timeText.text = seconds.ToString();
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                timeText.text = "";
                gameManager.timerFinished = true;
                timeText.gameObject.SetActive(false);
            }

        }
    }
}
