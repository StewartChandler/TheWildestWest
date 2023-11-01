using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HatStack : MonoBehaviour
{
    [SerializeField]
    private GameObject hatPrefab;

    private int startingNumHats = 3;

    private Stack<GameObject> hats = new Stack<GameObject>();
    Scene currentScene;

    // Start is called before the first frame update
    void Start()
    {
        resetHats();
        currentScene = SceneManager.GetActiveScene();

    }

    private void addHat()
    {
        currentScene = SceneManager.GetActiveScene();
        GameObject newHat = Instantiate(hatPrefab);
        pushHat(newHat);
    }

    public void pushHat(GameObject hat)
    {
        hat.transform.SetParent(transform, false);
        // if (currentScene.name == "PlayerSelect")
        // {
        //     hat.transform.position += new Vector3(0, 0.006f * hats.Count, 0);
        // }
        // else
        // {
        hat.transform.position += new Vector3(0, 0.4f * (hats.Count + 1), 0);
        // }
        hat.transform.rotation = Quaternion.Euler(
            Random.Range(-15.0f, 15.0f),
            Random.Range(-15.0f, 15.0f),
            Random.Range(-15.0f, 15.0f)
        );
        hats.Push(hat);
    }

    public void popHat(Vector3 displ)
    {
        GameObject hatObj = hats.Pop();

        hatObj.transform.SetParent(null, true);
        // Print the parent of the hat object
        hatObj.GetComponent<Hat>().launch(displ);
    }

    public void popAllHats()
    {
        while (hats.Count > 0)
        {
            popHat(Random.onUnitSphere);
        }
    }

    public void resetHats()
    {
        for (int i = 0; i < startingNumHats; i++)
        {
            // Debug.Log("adding hat");
            addHat();
        }
    }

    public int getNumHats()
    {
        return hats.Count;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
