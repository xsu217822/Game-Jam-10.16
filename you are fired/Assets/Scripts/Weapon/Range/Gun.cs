using UnityEngine;

public class Gun : WeaponBase
{
    [Header("Ranged Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;

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

        // ���Ŷ�������Ч
        animator?.SetTrigger("Attack");
        audioSource?.Play();

        // �����ӵ�
        if (bulletPrefab && firePoint)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = firePoint.right * bulletSpeed;
        }

        Debug.Log($"[gun] fired a bullet!");
    }
}

