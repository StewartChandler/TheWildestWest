using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndScreenDisplay : MonoBehaviour
{
    public Text mainText;
    // Start is called before the first frame update
    void Start()
    {
        mainText.text = "Nobody Wins :(";

        GameObject gameManagerObj = GameObject.Find("GameManager");
        if (gameManagerObj != null)
        {
            GameManager gameManager = gameManagerObj.GetComponent<GameManager>();
            for (int i = 0; i < 4; i++)
            {
                if (gameManager.playerScores[i] == 3)
                {
                    mainText.text = gameManager.players[i].name + " Wins!";
                }
            }

            SceneManager.MoveGameObjectToScene(GameObject.Find("Managers"), SceneManager.GetActiveScene());
        } 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
