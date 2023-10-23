using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Timer : MonoBehaviour
{
    public float timeRemaining = 91;
    public bool timerIsRunning = false;
    public TextMeshProUGUI timeText;
    private void Start()
    {
        // Starts the timer automatically
        timerIsRunning = true;
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
                timeText.text = "NT lil bro";
                timeText.color = Color.black;
            }
        }
    }
}