using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

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
        if (Input.GetMouseButton(0) 
            && WeaponController.Instance._canUse[WeaponTimerType.FIRE])
        {
            if (WeaponController.Instance.Fire() == false) return;

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

        if (Input.GetButtonDown("RELOAD"))
        {
            WeaponController.Instance.Reload();
        }
    }

    void FixedUpdate()
    {
    }

    GUIStyle style = new GUIStyle();
    private void OnGUI()
    {
        string ammo = WeaponController.Instance.Ammo.ToString();
        bool canFire = WeaponController.Instance._canUse[WeaponTimerType.FIRE];
        bool isReloading = WeaponController.Instance._canUse[WeaponTimerType.RELOAD];
        style.fontSize = 50;
        GUI.Label(new Rect(10, 60, 50, 50), ammo, style);
        GUI.Label(new Rect(10, 110, 50, 50), canFire ? "CAN SHOOT" : "SHOOTING", style);
        GUI.Label(new Rect(10, 160, 50, 50), isReloading ? "CAN RELOAD" : "RELOADING", style);
        if (GUI.Button(new Rect(10, 10, 100, 50), "Add Enemy"))
        {
            int zpos = Random.Range(6, -6);

            Vector3 pos = new Vector3(-30, 0.75f, zpos);

            GameObject obj = ObjectPool.GetObject(EnumData.EnemyType.DUMMY);

            obj.transform.position = pos;
        }
    }
}
