using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour
{
    FadeInOut fade;
    void Start()
    {
        fade = FindObjectOfType<FadeInOut>();
        fade.FadeOut();

    }


}
