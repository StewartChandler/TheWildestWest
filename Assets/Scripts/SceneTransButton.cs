using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransButton : MonoBehaviour
{
    public Button button;
    public string sceneName;

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(ChangeScene);
    }

    void ChangeScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
