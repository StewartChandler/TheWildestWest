using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenObjectThrow : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject itemToThrow;
    public bool spawnOn = true;
    public float spawnTime;
    public float spawnDelay;
    public float spawnDuration;
    public float throwSpeed;
    private Vector3 throwAngle = new Vector3(12f, 5f);
    void Start()
    {
        InvokeRepeating("SpawnObject", spawnTime, spawnDelay);
    }

    public void SpawnObject() 
    {
        if (spawnOn)
        {
            GameObject newObject = Instantiate(itemToThrow, transform.position, Quaternion.identity);
            Vector3 throwVector = transform.rotation * throwAngle;
            throwVector = throwVector.normalized * throwSpeed;
            newObject.GetComponentInChildren<Rigidbody>().velocity = throwVector;
        }
    }
}
