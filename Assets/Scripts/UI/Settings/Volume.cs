using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Volume : MonoBehaviour
{
    public AudioMixer audioMixer;
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", CalculateLog(volume));
    }

    private float CalculateLog(float x)
    {
        x = Mathf.Max(x, -79f);

        // Calculate y using the given equation
        float y = Mathf.Log(x + 80, 10) * 40 - 76;
        Debug.Log(y);

        return y;
    }
}
