using UnityEngine;

public class Sword : WeaponBase
{
    public float attackRadius = 1.5f;
    public LayerMask enemyLayer;

    [Header("Visual FX")]
    public GameObject slashFxPrefab;
    public float fxLifeTime = 0.15f;

    protected override void PerformAttack(Enemy target)
    {
        Vector2 center = transform.position;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            center,
            attackRadius,
            enemyLayer
        );

        foreach (Collider2D hit in hits)
        {
            var dmg = hit.GetComponent<IDamageable>();
            if (dmg != null && !dmg.IsDead)
                dmg.TakeDamage(baseDamage);
        }

        if (slashFxPrefab && target)
        {
            Vector2 dir = (target.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            GameObject fx = Instantiate(
                slashFxPrefab,
                transform.position,
                Quaternion.Euler(0, 0, angle)
            );

            Destroy(fx, fxLifeTime);
        }
    }
}
