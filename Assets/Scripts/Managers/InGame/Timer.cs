using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Timer : MonoBehaviour
{
    private float timeRemaining = 51.5f;
    public bool timerIsRunning = false;
    public TextMeshProUGUI timeText;
    public GameObject timerBorder;
    public GameObject leftBorder;
    public GameObject rightBorder;
    public GameObject topBorder;
    public GameObject bottomBorder;

    private float scale;
    private bool endRoundSound = false;

    GameManager gameManager;
    private void Start()
    {
        // Starts the timer automatically
        timerIsRunning = true;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.timeRanOut = false;
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
                // timer border
                if (timeRemaining < 10)
                {
                    timerBorder.gameObject.SetActive(true);
                    scale = (10 - timeRemaining) / 2;
                    leftBorder.transform.localScale = new Vector3(scale, 1, 110);
                    rightBorder.transform.localScale = new Vector3(scale, 1, 110);
                    topBorder.transform.localScale = new Vector3(200, 1, scale);
                    bottomBorder.transform.localScale = new Vector3(200, 1, scale);
                }
                if (timeRemaining < 3.5 && endRoundSound == false)
                {
                    StartCoroutine(EndRoundBeep());
                    endRoundSound = true;
                }
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                gameManager.timeRanOut = true;
                timerIsRunning = false;
                gameManager.EndRound();
                timeText.text = "";
                // timeText.text = "NT lil bro";
                // timeText.color = Color.black;

            }
        }
    }
    private IEnumerator EndRoundBeep()
    {
        AudioManager.instance.Play("TrainHorn2");
        yield return new WaitForSeconds(1f);
        AudioManager.instance.Play("TrainHorn2");
        yield return new WaitForSeconds(1f);
        AudioManager.instance.Play("TrainHorn2");
        yield return new WaitForSeconds(0.1f);
        AudioManager.instance.Play("TrainHorn3");
        yield return new WaitForSeconds(0.1f);
        AudioManager.instance.Play("TrainHorn3");
        yield return new WaitForSeconds(0.1f);
        AudioManager.instance.Play("TrainHorn3");
    }
}