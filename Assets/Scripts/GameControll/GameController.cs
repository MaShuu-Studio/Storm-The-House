using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private GameObject obj;
    [SerializeField] private GameObject EnemyObj;

    private void Awake()
    {
        cam = Camera.main;
        EnemyObj.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mpos = Input.mousePosition;
            mpos.z = cam.transform.position.z * -3;
            Vector3 dir = Vector3.Normalize(cam.ScreenToWorldPoint(mpos) - cam.transform.position);

            RaycastHit raycastHit;
            Physics.Raycast(cam.transform.position, dir, out raycastHit);

            Vector3 pos = Vector3.zero;

            if (raycastHit.transform != null)
               pos = raycastHit.point;

            obj.SetActive(true);
            obj.transform.position = pos;
        }

    }

    void FixedUpdate()
    {
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 50, 50), "Add Enemy"))
        {
            int zpos = Random.Range(6, -6);

            Vector3 pos = new Vector3(-30, 0.75f, zpos);

            GameObject obj = Instantiate(EnemyObj);
            obj.transform.position = pos;
            obj.SetActive(true);
        }
    }
}
