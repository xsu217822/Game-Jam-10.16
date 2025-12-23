using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public float range = 1.5f;
    public float baseDamage = 10f;
    public float attackSpeed = 1f;

    protected float lastAttackTime;

    public void Attack()
    {
        if (Time.time < lastAttackTime + 1f / attackSpeed)
            return;

        lastAttackTime = Time.time;
        PerformAttack();
    }

    protected abstract void PerformAttack();
}
