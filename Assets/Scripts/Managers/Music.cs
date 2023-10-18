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
        currentScene = SceneManager.GetActiveScene();
    }

    private void Update()
    {
        // delete object when curr scene = "DemoArena"
        if (currentScene.name == "DemoArena")
        {
            Destroy(gameObject);
        }
    }
}
