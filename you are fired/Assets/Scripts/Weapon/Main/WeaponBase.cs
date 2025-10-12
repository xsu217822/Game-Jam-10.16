using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Common")]
    public string displayName = "Weapon";
    public int baseDamage = 10; 
    public int cooldown = 1; 
    public LayerMask hitMask;
    public bool debugLogs;

    protected float lastFireTime = -999f;
    protected float lastAltTime = -999f;


    public bool CanFire
    {
        get
        {
            return Time.time >= lastFireTime + cooldown;
        }
    }

    //  ‘Õº≈Á∑¢
    public void TryFire(in FireInfo ctx)
    {
        if (!CanFire)
        {
            return;
        }
        lastFireTime = Time.time;
        DoFire(ctx);

        if (debugLogs)
        {
            Debug.Log($"[{displayName}] fire t={Time.time:F2}");
        }
    }

    protected abstract void DoFire(in FireInfo ctx); //≈Á∑¢
}
