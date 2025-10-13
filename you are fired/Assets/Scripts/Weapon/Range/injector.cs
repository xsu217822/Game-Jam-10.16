using UnityEngine;
using UnityEngine.Audio;

public class injector : WeaponBase
{
    [Header("Ranged Settings")]
    public GameObject injectorPrefab;
    public Transform firePoint;
    public float liquidSpeed = 20f;

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
        if (injectorPrefab && firePoint)
        {
            GameObject bullet = Instantiate(injectorPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = firePoint.right * liquidSpeed;
        }

        Debug.Log($"[Keyboard] fired a injector!");
    }
}

