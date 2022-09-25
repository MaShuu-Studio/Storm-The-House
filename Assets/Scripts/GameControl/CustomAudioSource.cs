using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CustomAudioSource : MonoBehaviour
{
    public bool State { get { return _audio.isPlaying; } }
    private AudioSource _audio;

    public void SetClip(string name, AudioClip clip)
    {
        _audio = GetComponent<AudioSource>();
        _audio.clip = clip;

        gameObject.name = name;
    }

    public void Play(float volume)
    {
        _audio.volume = volume;
        _audio.Play();
    }

    public void Pause()
    {
        _audio.Pause();
    }

    public void Stop()
    {
        _audio.Stop();
    }
}