using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [Header("Weapon")]
    public WeaponBase currentWeapon;

    [Header("Aim")]
    public Transform aimPivot;
    public Transform firePoint;

    [Header("Targeting")]
    public float targetRange = 8f;
    public float targetRefreshRate = 0.1f;

    private Enemy currentTarget;
    private float nextTargetTime;

    void Update()
    {
        if (!currentWeapon) return;

        UpdateTarget();
        AimAtTarget();

        if (currentTarget)
        {
            currentWeapon.Attack(currentTarget);
        }
    }

    void UpdateTarget()
    {
        if (Time.time < nextTargetTime) return;

        Vector3 origin = firePoint ? firePoint.position : transform.position;
        currentTarget = Enemy.FindNearest(origin, targetRange);

        nextTargetTime = Time.time + targetRefreshRate;
    }

    void AimAtTarget()
    {
        if (!currentTarget) return;

        Vector3 origin = firePoint ? firePoint.position : aimPivot.position;
        Vector2 dir = (currentTarget.transform.position - origin).normalized;

        if (aimPivot) aimPivot.right = dir;
        if (firePoint) firePoint.right = dir;
    }
}
