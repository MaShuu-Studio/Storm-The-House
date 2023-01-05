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

    private void Boom() // 다른 유닛에게도 데미지를 주는 공격
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
        float time = 0.07f; // 1초에 15번 정도 데미지
        while (true) // 죽을 떄 까지 데미지
        {
            while (time > 0)
            {
                time -= Time.deltaTime;
                yield return null;
            }
            Damage(burnDamage);
            time += 0.07f; // 시간이 밀렸을 경우를 대비 시간 초기화가 아닌 추가로 조정
        }
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
        animator.SetBool("ATTACK", !_isMoving);
    }

    // 애니메이션을 대체할 코루틴
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
