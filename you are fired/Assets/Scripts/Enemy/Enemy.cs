using UnityEngine;

public enum AttackWay { Melee, Ranged, None }

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    [Header("Fields")]
    public string Name = "Enemy";
    public float Health = 30f;
    public float Speed = 2f;
    public float AttackValue = 10f;
    public AttackWay AttackWay = AttackWay.Melee;

    [Header("Target")]
    public Transform target;
    public float stopDistance = 1.2f;

    private SpriteRenderer sprite;
    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 desiredDir;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // 自动寻找玩家
        if (!target)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) target = p.transform;
        }
    }

    void Update()
    {
        Navigation();
        EnemyMovement();
        EnemyAnimation();
    }

    // --- Functions ---
    public void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
            Die();
    }

    void EnemyMovement()
    {
        rb.linearVelocity = desiredDir * Speed;
    }

    void Navigation()
    {
        if (!target)
        {
            desiredDir = Vector2.zero;
            return;
        }

        Vector2 toTarget = target.position - transform.position;
        float dist = toTarget.magnitude;

        if (dist > stopDistance)
            desiredDir = toTarget.normalized;
        else
            desiredDir = Vector2.zero;
    }

    void EnemyAnimation()
    {
        if (rb.linearVelocity.x != 0)
            sprite.flipX = rb.linearVelocity.x < 0;

        if (animator)
            animator.SetFloat("Speed", rb.linearVelocity.magnitude);
    }

    void Die()
    {
        rb.linearVelocity = Vector2.zero;
        //if (animator) animator.SetTrigger("Die");
        Destroy(gameObject, 0.8f);
    }

    // --- 攻击逻辑：触碰造成伤害 ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 直接对玩家造成伤害
            var player = other.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(Mathf.RoundToInt(AttackValue));
            }

            // 如果希望只造成一次伤害（而不是持续接触伤害），可以用Destroy自己
            // Destroy(gameObject); 
        }
    }
}
