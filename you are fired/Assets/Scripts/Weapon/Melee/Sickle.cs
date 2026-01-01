using UnityEngine;

public class Sickle : WeaponBase
{
    [Header("Orbit (around player)")]
    public Transform orbitCenter;      // 不填就自动找 Player
    public float orbitRadius = 1.2f;   // 公转半径（离玩家多远）
    public float orbitSpeed = 180f;    // 公转速度（度/秒）
    public bool faceTangent = true;    // 镰刀朝向切线（看起来更像转圈）

    [Header("Damage")]
    public float hitRadius = 2f;       // 伤害判定半径（360°）
    public LayerMask enemyLayer;

    private float orbitAngleDeg;

    private void Awake()
    {
        // 如果你这个武器是挂在 Player/Weapons 下，一般 parent 就是玩家或武器根
        if (orbitCenter == null)
        {
            // 优先用父物体当中心（最稳）
            if (transform.parent != null) orbitCenter = transform.parent;
            else
            {
                // 兜底：按名字找
                GameObject p = GameObject.FindWithTag("Player");
                if (p != null) orbitCenter = p.transform;
            }
        }
    }

    private void Update()
    {
        if (orbitCenter == null) return;

        // 1) 公转：算圆周位置
        orbitAngleDeg += orbitSpeed * Time.deltaTime;
        float rad = orbitAngleDeg * Mathf.Deg2Rad;

        Vector2 offset = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * orbitRadius;
        transform.position = (Vector2)orbitCenter.position + offset;

        // 2) 视觉朝向（可选）：朝切线方向，让它看起来像在“转圈挥镰”
        if (faceTangent)
        {
            Vector2 tangent = new Vector2(-Mathf.Sin(rad), Mathf.Cos(rad)); // 圆的切线方向
            float z = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, z);
        }
        else
        {
            // 或者只自转（纯视觉）
            transform.Rotate(0, 0, orbitSpeed * Time.deltaTime);
        }
    }

    protected override void PerformAttack(Enemy target)
    {
        // 360° 判定：按你原来的逻辑
        Vector2 center = orbitCenter != null ? (Vector2)orbitCenter.position : (Vector2)transform.position;

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, hitRadius, enemyLayer);
        foreach (var hit in hits)
        {
            hit.GetComponent<IDamageable>()?.TakeDamage(baseDamage);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (orbitCenter != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(orbitCenter.position, orbitRadius);
        }

        Gizmos.color = Color.cyan;
        Vector3 c = orbitCenter != null ? orbitCenter.position : transform.position;
        Gizmos.DrawWireSphere(c, hitRadius);
    }
#endif
}
