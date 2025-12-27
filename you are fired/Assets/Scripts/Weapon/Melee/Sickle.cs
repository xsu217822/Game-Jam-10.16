using UnityEngine;

public class Sickle : WeaponBase
{
    [Header("Fan Attack")]
    public float radius = 2f;          // 攻击距离
    public float angle = 120f;         // 扇形总角度
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

            float enemyAngle = Vector2.Angle(dir, toEnemy);

            // 不在扇形角度内
            if (enemyAngle > angle * 0.5f)
                continue;

            hit.GetComponent<IDamageable>()
               ?.TakeDamage(baseDamage);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}
