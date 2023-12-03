using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;

    public Sound[] sounds;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.mixer;
        }
    }
    /**
        Takes an array of strings and plays a random sound from the array
    */
    public void Play(params string[] sound)
    {
        // get the length of the array
        int length = sound.Length;
        // get a random index
        int index = UnityEngine.Random.Range(0, length);
        // get the sound at the index
        string randomSound = sound[index];
        Sound s = Array.Find(sounds, item => item.name == randomSound);
        s.source.Play();
    }
    public void Stop(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        s.source.Stop();
    }

    public void PlayForSeconds(string sound, float seconds)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        s.source.Play();
        s.source.SetScheduledEndTime(AudioSettings.dspTime + seconds);
    }
}