using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    // Start is called before the first frame update

    Transform parentTransform;

    void Start()
    {
        Transform parentTransform = transform.parent;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = parentTransform.position;
        transform.rotation = parentTransform.rotation;
    }
}
