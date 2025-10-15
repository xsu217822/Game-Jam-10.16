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
            return;

        // Acquire nearest enemy within range
        Vector3 origin = firePoint ? (Vector3)firePoint.position : transform.position;
        float maxRange = range > 0f ? range : 9999f;
        Enemy target = Enemy.FindNearest(origin, maxRange);
        if (target == null)
            return;

        // Aim firePoint towards target
        Vector2 dir = ((Vector2)target.transform.position - (Vector2)origin).normalized;
        if (firePoint) firePoint.right = dir; else transform.right = dir;

        lastAttackTime = Time.time;

        // Play VFX/SFX
        animator?.SetTrigger("Attack");
        audioSource?.Play();

        // Fire bullet
        if (bulletPrefab && firePoint)
        {
            GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            var bullet = bulletObj.GetComponent<Bullet>();
            if (bullet != null)
            {
                var owner = GetComponentInParent<Player>() ? GetComponentInParent<Player>().gameObject : gameObject;
                bullet.Init(dir, bulletSpeed, baseDamage, owner);
            }
            else
            {
                // Fallback: push using rigidbody if Bullet script missing
                var rb = bulletObj.GetComponent<Rigidbody2D>();
                if (rb) rb.linearVelocity = firePoint.right * bulletSpeed;
            }
        }

        Debug.Log("[Keyboard] fired a bullet!");
    }
}
