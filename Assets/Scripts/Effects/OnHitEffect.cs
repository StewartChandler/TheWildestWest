using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class OnHitEffect : MonoBehaviour
{
    float tSinceCreated = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log("hey onhit effect created");
        VisualEffect effect = GetComponent<VisualEffect>();
        if (effect != null)
        {
            effect.Play();
        } else
        {
            Debug.Log("No visual effect could be played");
        }
    }

    void FixedUpdate()
    {
        tSinceCreated += Time.fixedDeltaTime;
        if (tSinceCreated > 5) {
            Destroy(gameObject);
        }
    }
}
