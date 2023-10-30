using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInOut : MonoBehaviour
{

    public CanvasGroup canvasGroup;
    public bool fadein = false;
    public bool fadeout = false;

    public float TimeToFade;

    // Update is called once per frame
    void Update()
    {
        if (fadein)
        {
            canvasGroup.alpha += Time.deltaTime / TimeToFade;
            if (canvasGroup.alpha >= 1)
            {
                fadein = false;
            }
        }

        if (fadeout)
        {
            canvasGroup.alpha -= Time.deltaTime / TimeToFade;
            if (canvasGroup.alpha <= 0)
            {
                fadeout = false;
            }
        }

    }
    public void FadeIn()
    {
        fadein = true;
    }
    public void FadeOut()
    {
        fadeout = true;
    }

}