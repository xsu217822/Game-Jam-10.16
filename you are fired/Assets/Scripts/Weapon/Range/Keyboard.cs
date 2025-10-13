using UnityEngine;

public class KeyboardWeapon : WeaponBase
{
    [Header("Ranged Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 14f;

    private float lastAttackTime;

    private void Start()
    {
        weaponType = WeaponType.Ranged;
    }

    public override void Attack()
    {
        if (Time.time < lastAttackTime + GetAttackInterval())
        {
            return;
        }

        lastAttackTime = Time.time;

        // 动画+音效
        animator?.SetTrigger("Attack");
        audioSource?.Play();

        // 发射子弹
        if (bulletPrefab && firePoint)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = firePoint.right * bulletSpeed;
        }

        Debug.Log($"[Keyboard] fired a bullet!");
    }
}
