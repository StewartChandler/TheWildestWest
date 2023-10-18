using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers instance;

    private void Awake()
    {
        // Check if an instance of the Managers script already exists
        if (instance != null)
        {
            // If an instance already exists, destroy the new one
            Destroy(gameObject);
        }
        else
        {
            // If no instance exists, set this as the instance and don't destroy it on scene load
            instance = this;
            DontDestroyOnLoad(gameObject);
            // foreach (Transform child in transform)
            // {
            //     DontDestroyOnLoad(child.gameObject);
            // }
        }
    }
}
