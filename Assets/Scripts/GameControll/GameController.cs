using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private GameObject obj;

    private void Awake()
    {
        cam = Camera.main;
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

            obj.transform.position = pos;
            Debug.Log("[MOUSE POS] " + mpos);
            Debug.Log("[RAY DIRECTION] " + dir);
            Debug.Log("[CONVERTED POS] " + pos);
        }

    }

    void FixedUpdate()
    {
    }
}
