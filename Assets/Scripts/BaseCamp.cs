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

    void Awake()
    {
        area = GetComponent<BoxCollider>();
        if (area == null)
            area = gameObject.AddComponent<BoxCollider>();

        area.isTrigger = true;
        area.center = new Vector3(-4, 0, 0);
        area.size = new Vector3(8, 2, 2);

        enemys = new Queue<GameObject>();
    }

    void Start()
    {
        supporter = new Dictionary<string, int>();
        coroutines = new Dictionary<string, IEnumerator>();

        foreach (string name in SupporterManager.Types)
        {
            supporter.Add(name, 0);
            coroutines.Add(name, null);
        }
    }

    void Update()
    {
        for (int i = 0; i < SupporterManager.Types.Count; i++)
        {
            string type = SupporterManager.Types[i];
            if (coroutines[type] == null && supporter[type] > 0)
                ActiveSupporter(type);
        }
    }

    public void AddSupporter(string type)
    {
        if (!supporter.ContainsKey(type)) return;
        
        supporter[type]++;
    }

    private void ActiveSupporter(string type)
    {
        if (!supporter.ContainsKey(type)) return;

        Debug.Log("[SYSTEM] ACTIVE SUPPROTER " + type);
        coroutines[type] = Active(type);
        StartCoroutine(coroutines[type]);
    }

    IEnumerator Active(string type)
    {
        float time = 0;
        float waitingTime;
        do
        {
            waitingTime = SupporterManager.Delay(type, supporter[type]);
            time += Time.deltaTime;
            yield return null;
        } while (time < waitingTime);

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
        style.fontSize = 0;

        for (int i = 0; i < SupporterManager.Types.Count; i++)
        {
            string type = SupporterManager.Types[i];
            if (GUI.Button(new Rect(1810, 10 + i * 60, 100, 50), type + " " + supporter[type]))
            {
                AddSupporter(type);
            }
        }
    }
}
