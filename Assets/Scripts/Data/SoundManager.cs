using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public static class SoundManager
{
    public static Dictionary<string, AudioClip> Sounds { get { return sounds; } }
    private static Dictionary<string, AudioClip> sounds;

    private static List<string> names;
    public static List<string> Names { get { return names; } }


    public static void Initialize()
    {
        sounds = new Dictionary<string, AudioClip>();
        names = new List<string>();

        List<AudioClip> list = DataManager.GetObjects<AudioClip>(DataManager.SoundPath);

        foreach(AudioClip clip in list)
        {
            string name = clip.name.ToUpper();
            sounds.Add(name, clip);
            names.Add(name);
        }
        Debug.Log("[SYSTEM] LOAD SOUND DATA");
    }
}
