using UnityEngine;

public class Knife : WeaponBase
{
    [Header("Attack")]
    public float hitRadius = 0.25f;
    public LayerMask enemyLayer;

    [Header("Fast Stab")]
    public float stabDistance = 0.12f;
    public float stabSpeed = 40f;

    [Header("Homing")]
    public float homingRadius = 3f;
    public float homingStrength = 10f;

    private Vector3 baseLocalPos;
    private float stabTimer;
    private Vector2 currentDir = Vector2.right;

    void Start()
    {
        baseLocalPos = transform.localPosition;
    }

    void Update()
    {
        // 找最近敌人（轻量）
        Enemy nearest = FindNearestEnemy();
        if (nearest != null)
        {
            Vector2 toEnemy =
                ((Vector2)nearest.transform.position - (Vector2)transform.position).normalized;

            currentDir = Vector2.Lerp(
                currentDir,
                toEnemy,
                homingStrength * Time.deltaTime
            );
        }

        // 朝向敌人
        float z = Mathf.Atan2(currentDir.y, currentDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, z);

        // 前后抖动
        stabTimer += Time.deltaTime * stabSpeed;
        float offset = Mathf.Sin(stabTimer) * stabDistance;
        transform.localPosition = baseLocalPos + (Vector3)(currentDir * offset);
    }

    protected override void PerformAttack(Enemy target)
    {
        // 高频、单刀近战
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            hitRadius,
            enemyLayer
        );

        foreach (var hit in hits)
        {
            hit.GetComponent<IDamageable>()
               ?.TakeDamage(baseDamage);
        }
    }

    Enemy FindNearestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            homingRadius,
            enemyLayer
        );

        Enemy nearest = null;
        float minDist = float.MaxValue;

        foreach (var h in hits)
        {
            float d = Vector2.Distance(transform.position, h.transform.position);
            if (d < minDist)
            {
                minDist = d;
                nearest = h.GetComponent<Enemy>();
            }
        }
        return nearest;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, homingRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitRadius);
    }
#endif
}
