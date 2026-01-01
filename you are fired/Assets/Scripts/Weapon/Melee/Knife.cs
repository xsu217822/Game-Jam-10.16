using UnityEngine;

public class Knife : WeaponBase
{
    public float range = 0.8f;
    public float spreadAngle = 40f; // 总角度
    public LayerMask enemyLayer;

    protected override void PerformAttack(Enemy target)
    {
        Vector2 center = transform.position;

        // 用玩家朝向（或 target 方向）
        Vector2 forward = target != null
            ? ((Vector2)target.transform.position - center).normalized
            : Vector2.right;

        int count = 5;
        float step = spreadAngle / (count - 1);
        float start = -spreadAngle * 0.5f;

        for (int i = 0; i < count; i++)
        {
            float angle = start + step * i;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * forward;

            RaycastHit2D hit = Physics2D.Raycast(
                center,
                dir,
                range,
                enemyLayer
            );

            if (hit.collider != null)
            {
                hit.collider.GetComponent<IDamageable>()
                   ?.TakeDamage(baseDamage);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
#endif
}
