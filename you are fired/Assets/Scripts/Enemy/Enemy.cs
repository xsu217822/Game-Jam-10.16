// Assets/Scripts/Enemy.cs
using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public event Action<Enemy> OnDied;
    private int hp;
    private float speed;
    private int exp;
    private Transform target;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private bool faceToMove = true;
    [SerializeField] private float stopDistance = 1.2f;
    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    public void Init(EnemyData d, Transform player)
    {
        hp = d.maxHealth; speed = d.moveSpeed; exp = d.expOnDie; target = player;
    }

    private void Update()
    {
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
            animator?.SetFloat("Speed", animSpeed); // >0 → Walk
        }
        else
        {
            animator?.SetFloat("Speed", 0f);
        }
    }

    public int KillAndGetExp() { OnDied?.Invoke(this); Destroy(gameObject); return exp; }
    public void TakeDamage(int dmg) { hp -= Mathf.Max(0, dmg); if (hp <= 0) KillAndGetExp(); }
}
