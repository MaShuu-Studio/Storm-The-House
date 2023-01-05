using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    private ParticleSystem ps;

    void Awake()
    {
        name = name.Substring(0, name.Length - 7).ToUpper();
        ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {        
        if (ps.IsAlive() == false)
        {
            ObjectPool.ReturnObject(gameObject.name, gameObject);
        }
    }
}
