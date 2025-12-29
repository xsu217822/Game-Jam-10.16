using UnityEngine;

public class Bottle : WeaponBase
{
    public GameObject bottleProjectilePrefab;
    public Transform firePoint;
    public float throwSpeed = 8f;

    protected override void PerformAttack(Enemy target)
    {
        Vector2 dir = firePoint.right;

        var go = Instantiate(
            bottleProjectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        go.GetComponent<BottleProjectile>()
          .Init(baseDamage, dir, throwSpeed);
    }
}
