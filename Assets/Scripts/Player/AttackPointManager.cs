using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class AttackPointManager : MonoBehaviour
{
    public static AttackPointManager Instance { get { return instance; } }
    private static AttackPointManager instance;

    [SerializeField] private AttackPoint prefab;

    List<AttackPoint> attackPoints;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        attackPoints = new List<AttackPoint>();
    }

    public AttackPoint MakeAttackPoint()
    {
        GameObject gobj = Instantiate(prefab.gameObject);
        AttackPoint newAttackPoint = gobj.GetComponent<AttackPoint>();

        attackPoints.Add(newAttackPoint);
        return newAttackPoint;
    }

    public void EnemyDamaged(EnemyObject enemy, GameObject attackPoint)
    {
        for (int i = 0; i < attackPoints.Count; i++)
        {
            if (attackPoint == attackPoints[i].gameObject)
            {
                attackPoints[i].EnemyDamaged(enemy);
                return;
            }
        }
    }
}
