using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    public event Action OnDied;
    private int hp;
    private float speed;
    private Transform target;

    public void ApplyData(EnemyData d)
    {
        hp = d ? d.maxHealth : 10;
        speed = d ? d.moveSpeed : 2f;
        target = FindObjectOfType<Player>()?.transform;
    }

    private void Update()
    {
        if (!target) return;
        var dir = (target.position - transform.position).normalized;
        transform.position += (Vector3)(dir * speed * Time.deltaTime);
    }

    public void TakeDamage(int dmg)
    {
        hp -= Mathf.Max(0, dmg);
        if (hp <= 0) { OnDied?.Invoke(); Destroy(gameObject); }
    }
}
