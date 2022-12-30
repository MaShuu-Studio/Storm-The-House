using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance { get { return instance; } }
    private static SoundController instance;

    [SerializeField] private SoundPool soundPool;
    [SerializeField] private AudioSource bgm;
    private List<CustomAudioSource> audios = new List<CustomAudioSource>();

    private float sfxVolume;
    private float bgmVolume;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Update()
    {
        for (int i = 0; i < audios.Count; i++)
        {
            if(!audios[i].State)
            {
                soundPool.ReturnAudio(audios[i].name, audios[i]);
                audios.RemoveAt(i);
                i--;
            }
        }
    }

    public void Initialize()
    {
        soundPool.Initialize();

        sfxVolume = 1;
        bgmVolume = 1;
    }

    public void PlayAudio(string name, Transform pos = null)
    {
        CustomAudioSource audio = soundPool.GetAudio(name);
        if (audio == null) return;

        audios.Add(audio);
        if (pos != null) audio.transform.position = pos.position;
        else audio.transform.position = Vector3.zero;

        audio.Play(sfxVolume);
    }

    public void PlayBGM(string name)
    {
        name = name.ToUpper();
        bgm.clip = SoundManager.GetSound(name);
        bgm.volume = bgmVolume;
        bgm.loop = true;

        bgm.Play();
    }

    public void PlayBGM()
    {
        bgm.Play();
    }

    public void StopBGM()
    {
        bgm.Stop();
    }

    public void PauseBGM()
    {
        bgm.Pause();
    }

    public void RestartBGM()
    {
        bgm.UnPause();
    }
}