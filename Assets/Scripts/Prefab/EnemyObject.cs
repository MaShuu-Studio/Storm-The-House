using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

public class EnemyObject : MonoBehaviour
{
    [SerializeField] private EnemyAttackArea _attackArea;
    private Enemy _enemy;

    private float _hp;

    private bool _meetBarricade = false;
    private bool _isMoving = true;

    void Awake()
    {
        name = name.Substring(0, name.Length - 7);
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
        _attackArea.Initialize(this, enemy.attackRange);

        Recover();
    }

    private void Recover()
    {
        _hp = _enemy.hp;
        slowAmount = 0;
    }

    private void Move()
    {
        transform.position += Vector3.right * _enemy.speed * Time.deltaTime * (1 - slowAmount);
    }

    IEnumerator attackCoroutine;

    private void Attack()
    {
        StopAttack();

        attackCoroutine = AttackTimer();
        StartCoroutine(AttackTimer());
    }

    float slowAmount = 0;
    public void Slow(float amount)
    {
        slowAmount = amount;
    }

    public void Down()
    {
        // 추후 애니메이션 관련 조절
        StartCoroutine(DownTestCoroutine());
    }

    // 애니메이션으로 캐릭의 다운을 조정하기 위한 함수
    private void CharacterDown(bool down)
    {
        _isMoving = !down;
    }

    // 애니메이션을 대체할 코루틴
    IEnumerator DownTestCoroutine()
    {
        CharacterDown(true);
        yield return new WaitForSeconds(1.5f);

        CharacterDown(false);
    }

    private void StopAttack()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }

    IEnumerator AttackTimer()
    {
        yield return new WaitForSeconds(_enemy.attackDelay);

        if (_meetBarricade)
        {
            Player.Instance.Damaged(_enemy.dmg);
            Attack();
        }
    }

    public void MeetBrricade(bool meet)
    {
        _meetBarricade = meet;
        _isMoving = !meet;

        if (meet)
            Attack();
        else StopAttack();
    }

    public void Damage(float dmg)
    {
        _hp -= dmg;
        Debug.Log("[SYSTEM] ENEMY DAMAGED" + dmg);

        if (_hp <= 0)
        {
            StopAttack();

            ObjectPool.ReturnObject(name, gameObject);

            Recover();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "AttackPoint")
        {
            AttackController.Instance.EnemyDamaged(this, other.gameObject);
        }
    }
}
