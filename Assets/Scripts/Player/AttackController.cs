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
        attackPoint = null;
    }

    void Start()
    {
        attackPoint = AttackPointManager.Instance.MakeAttackPoint();
        DontDestroyOnLoad(attackPoint.gameObject);
    }

    IEnumerator coroutine;

    public void SetDamage()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        coroutine = SetDMG();
        StartCoroutine(coroutine);
    }

    IEnumerator SetDMG()
    {
        while (attackPoint == null) yield return null;

        attackPoint.SetDamage(WeaponController.Instance.Damage);
    }

    public void Click(bool isFire)
    {
        bool canClick = (!isFire || (isFire && WeaponController.Instance.Fire()));


        if (canClick)
        {
            Vector3 pos = ClickPos();
            float remainTime = 0.1f;

            if (isFire)
            {
                // 무기의 공격이 남아있는 시간 입력
                // remainTim = WeaponController.Instance.
            }

            attackPoint.Attack(remainTime, pos);
        }
    }

    private Vector3 ClickPos()
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

        return pos;
    }
}
