using UnityEngine;

public class Gun : WeaponBase
{
    [Header("Ranged Settings")]
    public GameObject GunPrefab;
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

        // 播放动画和音效
        animator?.SetTrigger("Attack");
        audioSource?.Play();

        // 发射子弹
        if (GunPrefab && firePoint)
        {
            GameObject bullet = Instantiate(GunPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = firePoint.right * bulletSpeed;
        }

        Debug.Log($"[gun] fired a bullet!");
    }
}

