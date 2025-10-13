using UnityEngine;

public class Cleaver : WeaponBase
{
    [Header("Cleaver Settings")]
    public float swingRadius = 1.5f;
    public float swingAngle = 90f;
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
            return;
        }
        lastAttackTime = Time.time;

        animator?.SetTrigger("Attack");
        audioSource?.Play();

        Vector2 pos = transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, swingRadius, hitMask);
        foreach (var h in hits)
        {
            var target = h.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(baseDamage);
            }
        }
        Debug.Log($"[Cleaver] hit {hits.Length} enemies");
    }
}
