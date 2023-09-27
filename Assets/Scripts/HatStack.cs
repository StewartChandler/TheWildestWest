using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatStack : MonoBehaviour
{
    [SerializeField]
    private GameObject hatPrefab;

    private int startingNumHats = 3;

    private Stack<GameObject> hats = new Stack<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < startingNumHats; i++)
        {
            // Debug.Log("adding hat");
            addHat();
        }

    }

    private void addHat()
    {
        GameObject newHat = Instantiate(hatPrefab, transform);
        newHat.transform.position += new Vector3(0, 0.5f * hats.Count, 0);
        newHat.transform.rotation = Quaternion.Euler(
            Random.Range(-15.0f, 15.0f),
            Random.Range(-15.0f, 15.0f),
            Random.Range(-15.0f, 15.0f)
        );
        hats.Push(newHat);
    }

    public void pushHat(GameObject hat)
    {
        hat.transform.SetParent(transform, false);
        hat.transform.position += new Vector3(0, 0.7f * hats.Count, 0);
        hat.transform.rotation = Quaternion.Euler(
            Random.Range(-15.0f, 15.0f),
            Random.Range(-15.0f, 15.0f),
            Random.Range(-15.0f, 15.0f)
        );
        hats.Push(hat);
    }

    public void popHat()
    {
        GameObject hatObj = hats.Pop();

        hatObj.transform.SetParent(null, true);
        hatObj.GetComponent<Hat>().launch();
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
