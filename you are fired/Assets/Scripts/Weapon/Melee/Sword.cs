using UnityEngine;

public class Sword : WeaponBase
{
    [Header("Sword Settings")]
    public float stabDistance = 1f;   // �̻�����
    public LayerMask hitMask;

    private float lastAttackTime;

    private void Start()
    {
        weaponType = WeaponType.Melee;
    }

    public override void Attack()
    {
        if (Time.time < lastAttackTime + GetAttackInterval())
        {
            return; // �������δ�����޷�����
        }

        lastAttackTime = Time.time;

        // ���Ŷ�������Ч
        animator?.SetTrigger("Attack");
        audioSource?.Play();

        // �ж���ս����
        Vector2 origin = transform.position;
        Vector2 direction = transform.right;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, stabDistance, hitMask);
        if (hit.collider != null)
        {
            var target = hit.collider.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(baseDamage);
            }

            Debug.Log($"[Sword] stabbed {hit.collider.name} for {baseDamage} damage");
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * stabDistance);
    }
}
