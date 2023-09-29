using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float health = 1.0f;

    private float def_width;
    private RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        def_width = rectTransform.sizeDelta.x;
    }

    // Update is called once per frame
    void Update()
    {
        health = health > 0f ? (health < 1f ? health : 1f) : 0f;

        rectTransform.sizeDelta = new Vector2(def_width * health, rectTransform.sizeDelta.y);
    }

    void End()
    {
        rectTransform.sizeDelta = new Vector2(def_width, rectTransform.sizeDelta.y);
    }
}
