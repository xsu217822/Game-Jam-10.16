using UnityEngine;

public class Bottle : WeaponBase
{
    [Header("Ranged Settings")]
    public GameObject bottlePrefab;
    public Transform firePoint;
    public float bottleSpeed = 7f;

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
        if (bottlePrefab && firePoint)
        {
            GameObject bullet = Instantiate(bottlePrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = firePoint.right * bottleSpeed;
        }

        Debug.Log($"[Keyboard] throw a bottle!");
    }

}
