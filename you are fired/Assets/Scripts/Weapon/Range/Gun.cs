using UnityEngine;

public class Gun : WeaponBase
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;

    protected override void PerformAttack(Enemy target)
    {
        Debug.Log("Gun Attack Fired");

        var go = Instantiate(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation
        );

        go.GetComponent<Bullet>()
          .Init(baseDamage, firePoint.right, bulletSpeed);
    }
}
