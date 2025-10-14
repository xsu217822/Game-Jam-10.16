// Assets/Scripts/Enemy.cs
using System;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public event Action<Enemy> OnDied;
    [Header("Stat")]
    [SerializeField] private int maxHp = 30;
    [SerializeField] private float speed = 2f;
    [SerializeField] private int expOnDie = 5;

    private int hp;
    private Transform target;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private bool faceToMove = true;
    [SerializeField] private float stopDistance =0.1f;

    [Header("Hit Feedback")]
    [SerializeField] private float hitInvincibleSeconds = 0.15f;
    private float lastHitTime = -999f;

    public int Exp => expOnDie;
    public bool IsDead { get; private set; }

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        hp = maxHp;
    }

    public void Init(EnemyData d, Transform player)
    {
        maxHp = d.maxHealth;
        hp = maxHp;
        speed = d.moveSpeed;
        expOnDie = d.expOnDie;
        target = player;
    }

    private void Update()
    {
        if (IsDead) return;

        if (!target)
        {
            animator?.SetFloat("Speed", 0f);
            return;
        }

        var to = (target.position - transform.position);
        float dist = to.magnitude;

        if (dist > stopDistance)
        {
            var dir = to / dist; // normalized
            var delta = dir * speed * Time.deltaTime;
            transform.position += delta;

            if (faceToMove && Mathf.Abs(dir.x) > 0.001f)
                transform.localScale = new Vector3(Mathf.Sign(dir.x), 1, 1);

            float animSpeed = delta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
            animator?.SetFloat("Speed", animSpeed);
        }
        else
        {
            animator?.SetFloat("Speed", 0f);
        }
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        // 短暂无敌
        if (Time.time < lastHitTime + hitInvincibleSeconds) return;
        lastHitTime = Time.time;

        int dmg = Mathf.CeilToInt(Mathf.Max(0f, amount)); 
        hp -= dmg;

        animator?.SetTrigger("Hit");


        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;

        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        // 通知订阅者
        OnDied?.Invoke(this);

        Destroy(gameObject, 0.15f);
    }

    public int KillAndGetExp()
    {
        if (!IsDead) Die();
        Destroy(gameObject);
        return expOnDie;
    }
}
