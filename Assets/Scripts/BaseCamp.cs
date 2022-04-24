using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BaseCamp : MonoBehaviour
{
    [SerializeField] private AttackPoint attackPoint;
    private BoxCollider area;
    private Dictionary<string, int> supporter;
    private Queue<GameObject> enemys;

    void Awake()
    {
        area = GetComponent<BoxCollider>();
        if (area == null)
            area = gameObject.AddComponent<BoxCollider>();

        area.isTrigger = true;
        area.center = new Vector3(-4, 0, 0);
        area.size = new Vector3(8, 2, 2);

        supporter = new Dictionary<string, int>()
        {
            {"GUNMAN", 0 },
            {"REPAIRMAN", 0 },
        };

        enemys = new Queue<GameObject>();
    }

    IEnumerator Fire(string type)
    {
        float time = SupporterManager.Delay(type, supporter[type]);
        yield return new WaitForSeconds(time);

        if (enemys.Count > 0)
        {
            GameObject target;
            do
            {
                target = enemys.Peek();

                if (target == null || target.activeSelf == false) enemys.Dequeue();
                else break;
            } 
            while (enemys.Count > 0);

            if (enemys.Count > 0)
            {
                attackPoint.Attack(0.1f, target.transform.position);
                StartCoroutine(Fire(type));
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            enemys.Enqueue(other.gameObject);

            if (enemys.Count == 1)
                StartCoroutine(Fire("GUNMAN"));
        }
    }

    GUIStyle style = new GUIStyle();
    private void OnGUI()
    {
        style.fontSize = 50;
        GUI.Label(new Rect(10, 210, 50, 50), "GUNMAN: " + supporter["GUNMAN"].ToString(), style);
        if (GUI.Button(new Rect(120, 10, 100, 50), "Add GUNMAN"))
        {
            supporter["GUNMAN"]++;
        }
    }
}
