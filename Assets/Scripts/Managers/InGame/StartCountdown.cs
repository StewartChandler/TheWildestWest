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
        AudioManager.instance.StopAll();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Pause the game
        Time.timeScale = 0;
        timeRemaining = 5;

        // Starts the timer automatically
        timerIsRunning = true;

        // play the sounds
        StartCoroutine(playHorns());

        // Stop the current music
        AudioManager.instance.Stop("Music1");
        // AudioManager.instance.Stop("Music2");

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
                // if (gameManager.currRound != 3)
                // {
                AudioManager.instance.Play("Music1");
                // }
                // else
                // {
                //     AudioManager.instance.Play("Music3");
                // }
            }
        }
    }
    private IEnumerator playHorns()
    {
        // start the train chugging
        AudioManager.instance.PlayForSeconds("TrainChugging", timeRemaining);
        yield return new WaitForSecondsRealtime(0.26f);
        AudioManager.instance.Play("TrainHorn2");
        // wait until timeRemaining is 3
        yield return new WaitForSecondsRealtime(1f);
        // play the first horn
        AudioManager.instance.Play("TrainHorn2");
        // wait for 1 second
        yield return new WaitForSecondsRealtime(1f);
        // play the second horn
        AudioManager.instance.Play("TrainHorn2");
        // wait for 1 second
        yield return new WaitForSecondsRealtime(1f);
        // play the third horn
        AudioManager.instance.Play("TrainHorn");

    }
}
