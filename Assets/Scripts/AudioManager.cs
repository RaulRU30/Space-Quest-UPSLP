using UnityEngine.Audio;
using System;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0, 1)]
    public float volume = 1;
    [Range(-3, 3)]
    public float pitch = 1;
    public bool loop = false;
    public bool playOnAwake = false;
    public AudioSource source;

    public Sound()
    {
        volume = 1;
        pitch = 1;
        loop = false;
    }
}

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;
    void Awake()
    {
        instance = this;

        foreach (Sound s in sounds)
        {
            if (!s.source)
                s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.playOnAwake = s.playOnAwake;
            if (s.playOnAwake)
                s.source.Play();

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
        
        PlayWithFade("bgm");
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }

        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        s.source.Stop();
    }
    
    public void PlayWithFade(string name, float fadeDuration = 1.5f)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }

        StartCoroutine(FadeIn(s, fadeDuration));
    }

    private IEnumerator FadeIn(Sound s, float duration)
    {
        if (s.source == null)
            yield break;

        s.source.volume = 0f;
        s.source.Play();

        float targetVolume = s.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            s.source.volume = Mathf.Lerp(0f, targetVolume, elapsed / duration);
            yield return null;
        }

        s.source.volume = targetVolume;
    }
    
    public void StopWithFade(string name, float fadeDuration = 1f)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null || s.source == null)
        {
            Debug.LogWarning("Sound not found: " + name);
            return;
        }

        StartCoroutine(FadeOutAndStop(s, fadeDuration));
    }

    private IEnumerator FadeOutAndStop(Sound s, float duration)
    {
        float startVolume = s.source.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            s.source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        s.source.Stop();
        s.source.volume = s.volume; // reset to default volume for next time
    }


}