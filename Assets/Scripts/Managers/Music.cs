using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Music : MonoBehaviour
{
    private AudioSource _audioSource;
    Scene currentScene;
    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // delete object when curr scene = "DemoArena"
        currentScene = SceneManager.GetActiveScene();
        Debug.Log(currentScene.name);
        if (currentScene.name != "StartScene" && currentScene.name != "PlayerSelect")
        {
            Destroy(gameObject);
        }
    }
}
