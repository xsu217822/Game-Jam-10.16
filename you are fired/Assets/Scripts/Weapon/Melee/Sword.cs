using UnityEngine;

public class Sword : WeaponBase
{
    [Header("Sword Settings")]
    public float stabDistance = 1f;   // 刺击距离
    public LayerMask hitMask;

    private float lastAttackTime;

    private void Start()
    {
        weaponType = WeaponType.Melee;
    }

    public override void Attack()
    {
        if (Time.time < lastAttackTime + GetAttackInterval())
        {
            return; // 攻击间隔未到，无法攻击
        }

        lastAttackTime = Time.time;

        // 播放动画和音效
        animator?.SetTrigger("Attack");
        audioSource?.Play();

        // 判定近战攻击
        Vector2 origin = transform.position;
        Vector2 direction = transform.right;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, stabDistance, hitMask);
        if (hit.collider != null)
        {
            var target = hit.collider.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(baseDamage);
            }

            Debug.Log($"[Sword] stabbed {hit.collider.name} for {baseDamage} damage");
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * stabDistance);
    }
}
