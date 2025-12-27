using UnityEngine;

public class Knife : WeaponBase
{
    public float radius = 0.6f;
    public float angle = 40f;
    public LayerMask enemyLayer;

    protected override void PerformAttack(Enemy target)
    {
        Vector2 center = (Vector2)transform.position;
        Vector2 dir = ((Vector2)target.transform.position - center).normalized;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            center,
            radius,
            enemyLayer
        );

        foreach (var hit in hits)
        {
            Vector2 toEnemy =
                ((Vector2)hit.transform.position - center).normalized;

            if (Vector2.Angle(dir, toEnemy) > angle * 0.5f)
                continue;

            hit.GetComponent<IDamageable>()
               ?.TakeDamage(baseDamage);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}
