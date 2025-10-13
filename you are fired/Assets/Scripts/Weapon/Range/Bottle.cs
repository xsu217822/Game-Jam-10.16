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

        // ���Ŷ�������Ч
        animator?.SetTrigger("Attack");
        audioSource?.Play();

        // �����ӵ�
        if (bottlePrefab && firePoint)
        {
            GameObject bullet = Instantiate(bottlePrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = firePoint.right * bottleSpeed;
        }

        Debug.Log($"[Keyboard] throw a bottle!");
    }

}
