using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private LineRenderer lr;
    // Start is called before the first frame update
    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    float time = 0;
    void Update()
    {
        if (gameObject.activeSelf)
        {
            time += Time.deltaTime;
            if (time > 0.3f) gameObject.SetActive(false);
        }
    }

    public void Setposition(Vector3 start, Vector3 end)
    {
        gameObject.SetActive(true);
        start.x -= 0.5f;
        start.y += 0.255f;
        start.z -= 0.5f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        time = 0;
    }
}
