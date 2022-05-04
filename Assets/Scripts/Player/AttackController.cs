using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

public class AttackController : MonoBehaviour
{
    public static AttackController Instance { get { return instance; } }
    private static AttackController instance;

    private AttackPoint attackPoint;

    private Camera cam;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        cam = Camera.main;
    }

    void Start()
    {
        attackPoint = AttackPointManager.Instance.MakeAttackPoint();
        DontDestroyOnLoad(attackPoint.gameObject);
    }

    public void SetDamage()
    {
        attackPoint.SetDamage(WeaponController.Instance.Damage);
    }

    public void Fire()
    {
        if (WeaponController.Instance.Fire())
        {
            Vector3 mpos = Input.mousePosition;
            mpos.z = cam.transform.position.z * -4;
            Vector3 dir = Vector3.Normalize(cam.ScreenToWorldPoint(mpos) - cam.transform.position);

            RaycastHit raycastHit;
            LayerMask mask = LayerMask.GetMask("Floor") | LayerMask.GetMask("Enemy");
            Physics.Raycast(cam.transform.position, dir, out raycastHit, Mathf.Infinity, mask);

            Vector3 pos = Vector3.zero;

            if (raycastHit.transform != null)
                pos = raycastHit.point;

            attackPoint.Attack(0.5f, pos);
        }
    }
}
