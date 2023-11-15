using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainCollisionRefer : MonoBehaviour
{
    void OnColliderEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Hit!");
            collision.gameObject.GetComponent<PlayerController>().takeDamage(-10 * (transform.position - collision.gameObject.transform.position));
        }
        Debug.Log(collision.gameObject.tag);
    }
}
