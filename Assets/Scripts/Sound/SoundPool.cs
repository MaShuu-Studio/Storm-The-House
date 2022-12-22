using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    public Dictionary<string, Queue<CustomAudioSource>> Pool { get { return _pool; } }
    private Dictionary<string, Queue<CustomAudioSource>> _pool;
    protected Dictionary<string, Transform> _poolParent;

    public void Initialize()
    {
        _pool = new Dictionary<string, Queue<CustomAudioSource>>();
        _poolParent = new Dictionary<string, Transform>();

        for (int i = 0; i < SoundManager.SoundsSize; i++)
        {
            string name = SoundManager.Names[i];
            _pool.Add(name, new Queue<CustomAudioSource>());
            GameObject parent = new GameObject(name);
            parent.transform.SetParent(this.transform);
            _poolParent.Add(name, parent.transform);

            for(int j = 0; j < 5; j++)
                CreateNewAudio(name);
        }
    }
    
    private void CreateNewAudio(string name)
    {
        GameObject gbo = Instantiate(prefab);
        CustomAudioSource audio = gbo.GetComponent<CustomAudioSource>();
        AudioClip clip = SoundManager.GetSound(name);

        gbo.transform.SetParent(_poolParent[name]);
        audio.SetClip(name, clip);
        _pool[name].Enqueue(audio);
    }

    public CustomAudioSource GetAudio(string name)
    {
        name = name.ToUpper();
        if (_pool.ContainsKey(name) == false) return null;

        if (_pool[name].Count <= 0)
            CreateNewAudio(name);

        CustomAudioSource audio = _pool[name].Dequeue();
        audio.gameObject.SetActive(true);

        return audio;
    }

    public void ReturnAudio(string name, CustomAudioSource audio)
    {
        audio.gameObject.transform.SetParent(_poolParent[name]);
        audio.gameObject.SetActive(false);

        _pool[name].Enqueue(audio);
    }
}