using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController Instacne { get { return instance; } }
    private static UIController instance;

    [Header("Base Object")]
    [SerializeField] private GameObject canvas;

    [Header("Data")]
    [SerializeField] private Slider ammoSlider;
    [SerializeField] private TextMeshProUGUI ammoText;
    private int maxAmmo;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;
    private int maxHp;

    [SerializeField] private TextMeshProUGUI moneyText;

    [Space]
    [SerializeField] private TextMeshProUGUI baseText;

    void Awake()
    {
        if (Instacne != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(canvas);
    }

    public void UpdateMoney(int money)
    {
        moneyText.text = "$" + money;
    }

    public void SetAmmo(int cur, int max)
    {
        if (rlcoroutine != null)
        {
            StopCoroutine(rlcoroutine);
        }

        maxAmmo = max;
        ammoSlider.maxValue = max;


        UpdateAmmo(cur);
    }

    public void UpdateAmmo(int cur)
    {
        ammoText.text = cur + "/" + maxAmmo;
        ammoSlider.value = cur;
    }

    IEnumerator rlcoroutine = null;

    public void Reload(float time)
    {
        if (rlcoroutine != null)
        {
            StopCoroutine(rlcoroutine);
        }

        rlcoroutine = Reloading(time);
        StartCoroutine(rlcoroutine);
    }

    IEnumerator Reloading(float time)
    {
        float t = 0;
        while (time > t)
        {
            t += Time.deltaTime;
            if (t > time) t = time;
            ammoSlider.value = t / time * maxAmmo;
            yield return null;
        }
    }

    public void SetHP(int cur, int max)
    {
        maxHp = max;
        hpSlider.maxValue = max;

        UpdateHP(cur);
    }
    public void UpdateHP(int cur)
    {
        hpText.text = cur + "/" + maxHp;
        hpSlider.value = cur;
    }

    public void SetSupporter(int shield, int gun, int repair)
    {
        baseText.text = shield + "%" + "\n" + gun.ToString() + "\n" + repair.ToString();
    }
}