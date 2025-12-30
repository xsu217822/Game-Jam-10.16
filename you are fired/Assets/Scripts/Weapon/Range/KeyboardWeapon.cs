using UnityEngine;

public class KeyboardWeapon : WeaponBase
{
    [Header("Projectile")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 12f;

    [Header("Keyboard Settings")]
    public int lettersPerShot = 3;
    public float spreadAngle = 15f;    // É¢µ¯

    protected override void PerformAttack(Enemy target)
    {
        if (target == null) return;

        Vector2 baseDir = firePoint.right;

        for (int i = 0; i < lettersPerShot; i++)
        {
            float angleOffset = Random.Range(-spreadAngle, spreadAngle);
            Vector2 dir =
                Quaternion.Euler(0, 0, angleOffset) * baseDir;

            GameObject go = Instantiate(
                bulletPrefab,
                firePoint.position,
                Quaternion.identity
            );

            KeyboardBullet bullet =
                go.GetComponent<KeyboardBullet>();

            if (bullet != null)
            {
                bullet.Init(baseDamage, dir, bulletSpeed);
            }
        }
    }
}
