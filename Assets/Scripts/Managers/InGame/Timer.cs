using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Timer : MonoBehaviour
{
    public float timeRemaining = 91;
    public bool timerIsRunning = false;
    public TextMeshProUGUI timeText;

    GameManager gameManager;
    private void Start()
    {
        // Starts the timer automatically
        timerIsRunning = true;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                float seconds = Mathf.FloorToInt(timeRemaining);
                if (timeRemaining < 10)
                {
                    timeText.color = Color.red;
                    timeText.text = "0" + seconds.ToString();
                }
                else
                {
                    timeText.color = Color.white;
                    timeText.text = seconds.ToString();
                }
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
                gameManager.EndRound();
                timeText.text = "Most Hats Wins!";
                // timeText.text = "NT lil bro";
                // timeText.color = Color.black;

            }
        }
    }
}