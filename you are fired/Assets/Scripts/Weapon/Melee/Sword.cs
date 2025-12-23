using UnityEngine;

public class Sword : WeaponBase
{
    protected override void PerformAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            range
        );

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var dmg))
            {
                dmg.TakeDamage(baseDamage);
            }
        }
    }
}
