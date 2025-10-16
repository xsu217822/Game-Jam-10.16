using UnityEngine;

public class KeyboardWeapon : WeaponBase
{
    [Header("Ranged Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 14f;

    [Header("Auto Targeting")]
    public bool autoFire = true;          // auto fire by cooldown
    public Transform aimOrigin;           // optional; fallback to firePoint / self

    private float lastAttackTime;

    private void Start()
    {
        weaponType = WeaponType.Ranged;
        if (aimOrigin == null) aimOrigin = firePoint != null ? firePoint : transform;
    }

    private void Update()
    {
        if (autoFire) Attack();
    }

    public override void Attack()
    {
        if (Time.time < lastAttackTime + GetAttackInterval())
        {
            return;
        }

        // Acquire nearest target within weapon range
        Vector3 origin = (aimOrigin ? aimOrigin.position : transform.position);
        float maxRange = range > 0f ? range : 9999f;
        Enemy target = Enemy.FindNearest(origin, maxRange);
        if (target == null) return;

        // Aim at target
        Vector2 dir = ((Vector2)target.transform.position - (Vector2)origin).normalized;
        if (firePoint) firePoint.right = dir;
        else transform.right = dir;

        lastAttackTime = Time.time;

        // 播放动画和音效
        animator?.SetTrigger("Attack");
        audioSource?.Play();

        // 发射子弹
        if (bulletPrefab && firePoint)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(Vector3.forward, Vector3.right));
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = (Vector2)(firePoint.right) * bulletSpeed;
        }

        Debug.Log($"[Keyboard] fired a bullet!");
    }
}
