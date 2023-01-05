using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

public class EnemyObject : MonoBehaviour
{
    [SerializeField] private EnemyAttackArea _attackArea;
    [SerializeField] private GameObject burnedEffect;
        
    private Animator animator;
    private Enemy _enemy;
    public string type { get; private set; }

    private float _hp;

    private bool _meetBarricade = false;
    private bool _isMoving = true;

    void Awake()
    {
        name = name.Substring(0, name.Length - 7).ToUpper();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isMoving && !_meetBarricade)
            Move();
    }

    public void Initialize()
    {
        Enemy enemy = EnemyManager.Enemies[name];

        _enemy = enemy;
        type = enemy.type;
        _attackArea.Initialize(this, enemy.attackrange);

        Recover();
    }

    private void Recover()
    {
        _hp = _enemy.hp;
        slowAmount = 0;
        if (burnCoroutine != null)
        {
            burnDamage = 0;
            burnedEffect.SetActive(false);
            StopCoroutine(burnCoroutine);
            burnCoroutine = null;
        }
        MeetBrricade(false);
    }

    private void Move()
    {
        transform.position += Vector3.right * _enemy.speed * Time.deltaTime * (1 - slowAmount);
    }

    IEnumerator attackCoroutine;

    private void Attack()
    {
        SoundController.Instance.PlayAudio("ENEMY " + _enemy.name.ToUpper(), transform);
        if (_meetBarricade) Player.Instance.Damaged(_enemy.dmg);
    }

    private void Boom() // �ٸ� ���ֿ��Ե� �������� �ִ� ����
    {
        Attack();
        AttackController.Instance.EnemyBoom(transform.position);
        Damage(9999);
    }
    float slowAmount = 0;
    public void Slow(float amount)
    {
        slowAmount = amount;
    }

    private float burnDamage = 0;
    private IEnumerator burnCoroutine;
    public void Burn(float dmg)
    {
        burnDamage = dmg;
        if (burnCoroutine == null && burnedEffect != null)
        {
            burnedEffect.SetActive(true);
            burnCoroutine = Burning();
            StartCoroutine(burnCoroutine);
        }
    }

    private IEnumerator Burning()
    {
        float time = 0.07f; // 1�ʿ� 15�� ���� ������
        while (true) // ���� �� ���� ������
        {
            while (time > 0)
            {
                time -= Time.deltaTime;
                yield return null;
            }
            Damage(burnDamage);
            time += 0.07f; // �ð��� �з��� ��츦 ��� �ð� �ʱ�ȭ�� �ƴ� �߰��� ����
        }
    }

    public void Down()
    {
        // ���� �ִϸ��̼� ���� ����
        StartCoroutine(DownTestCoroutine());
    }

    // �ִϸ��̼����� ĳ���� �ٿ��� �����ϱ� ���� �Լ�
    private void CharacterDown(bool down)
    {
        _isMoving = !down;
        animator.SetBool("ATTACK", !_isMoving);
    }

    // �ִϸ��̼��� ��ü�� �ڷ�ƾ
    IEnumerator DownTestCoroutine()
    {
        CharacterDown(true);
        yield return new WaitForSeconds(1.5f);

        CharacterDown(false);
    }

    public void MeetBrricade(bool meet)
    {
        _meetBarricade = meet;
        _isMoving = !meet;
        animator.SetBool("ATTACK", !_isMoving);
    }

    public void Damage(float dmg)
    {
        _hp -= dmg;
        Debug.Log($"[SYSTEM] {gameObject.name} DAMAGED {dmg}| HP: {_hp}");

        if (_hp <= 0)
        {
            ObjectPool.ReturnObject(name, gameObject);

            Recover();
        }
    }
}
