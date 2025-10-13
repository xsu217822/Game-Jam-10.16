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

    public void Init(EnemyData d, Transform player)
    {
        hp = d.maxHealth; speed = d.moveSpeed; exp = d.expOnDie; target = player;
    }

    private void Update()
    {
        if (!target) return;
        var dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }

    public int KillAndGetExp() { OnDied?.Invoke(this); Destroy(gameObject); return exp; }
    public void TakeDamage(int dmg) { hp -= Mathf.Max(0, dmg); if (hp <= 0) KillAndGetExp(); }
}
