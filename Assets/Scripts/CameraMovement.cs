using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 Offset = new Vector3(0, 2, -15);
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        gameObject.transform.position = player.transform.position;
        gameObject.transform.Translate(Offset);

    }
}
