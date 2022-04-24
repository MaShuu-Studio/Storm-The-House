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

    private Dictionary<string, IEnumerator> coroutines;
    private List<string> keys;

    void Awake()
    {
        area = GetComponent<BoxCollider>();
        if (area == null)
            area = gameObject.AddComponent<BoxCollider>();

        area.isTrigger = true;
        area.center = new Vector3(-4, 0, 0);
        area.size = new Vector3(8, 2, 2);

        enemys = new Queue<GameObject>();

        supporter = new Dictionary<string, int>()
        {
            {"GUNMAN", 0 },
            {"REPAIRMAN", 0 },
        };

        coroutines = new Dictionary<string, IEnumerator>();
        keys = new List<string>();
        foreach (string key in supporter.Keys)
        {
            keys.Add(key);
            coroutines.Add(key, null);
        }
    }

    void Update()
    {
        for (int i = 0; i < keys.Count; i++)
        {
            string type = keys[i];
            if (coroutines[type] == null && supporter[type] > 0)
                ActiveSupporter(type);
        }
    }

    private void ActiveSupporter(string type)
    {
        Debug.Log("[SYSTEM] ACTIVE SUPPROTER " + type);
        coroutines[type] = Active(type);
        StartCoroutine(coroutines[type]);
    }

    IEnumerator Active(string type)
    {
        float time = SupporterManager.Delay(type, supporter[type]);
        yield return new WaitForSeconds(time);
        
        GameObject target;
        Vector3 pos = Vector3.zero; 
            
        while (enemys.Count > 0)
        {
            target = enemys.Peek();
            
            if (target == null || target.activeSelf == false) enemys.Dequeue();
            else
            {
                pos = target.transform.position;
                attackPoint.Attack(0.1f, pos);
                break;
            }
        } 

        if (enemys.Count > 0)
            ActiveSupporter(type);
        else
            coroutines[type] = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            enemys.Enqueue(other.gameObject);
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
