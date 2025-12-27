using UnityEngine;

public class Sword : WeaponBase
{

    private Enemy cachedTarget;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInParent<Animator>();
    }

    protected override void PerformAttack(Enemy target)
    {
        if (!target) return;

        cachedTarget = target;

        animator.SetTrigger("Attack");
    }

    // ¶¯»­Ö¡
    public void DoHit()
    {
        if (!cachedTarget) return;

        float dist = Vector2.Distance(
            transform.position,
            cachedTarget.transform.position
        );

        if (dist > range) return;

        IDamageable dmg = cachedTarget.GetComponent<IDamageable>();
        if (dmg != null && !dmg.IsDead)
        {
            dmg.TakeDamage(baseDamage);
            Debug.Log("Sword Hit");
        }
    }
}
