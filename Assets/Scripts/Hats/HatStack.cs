using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class HatStack : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> hatVariants;

    [SerializeField]
    private GameObject hatPrefab;

    private int startingNumHats = 3;
    private int pIndex;

    private Stack<GameObject> hats = new Stack<GameObject>();
    Scene currentScene;

    // Start is called before the first frame update
    void Start()
    {
        currentScene = SceneManager.GetActiveScene();

        // I hate this code lol
        // PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();

    }

    private void Awake()
    {
        PlayerInput player = GetComponentInParent<PlayerInput>();
        pIndex = player.playerIndex;

        resetHats();
    }

    private void addHat()
    {
        currentScene = SceneManager.GetActiveScene();
        GameObject newHat = Instantiate(hatVariants[pIndex]);
        pushHat(newHat);
    }

    public void pushHat(GameObject hat)
    {
        hat.transform.SetParent(transform, false);
        hat.transform.position += new Vector3(0, 0.3f * hats.Count + 0.2f, 0);
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
