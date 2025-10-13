using UnityEngine;

public enum WeaponType
{
    Melee,
    Ranged
}

/// <summary>
/// WeaponBase for all weapon types (Melee and Ranged)  
/// </summary>
public class WeaponBase : MonoBehaviour
{
    [Header("Weapon Type")]
    public WeaponType weaponType;

    [Header("Weapon Stats")]
    public float range = 1.5f;       // ¹¥»÷¾àÀë
    public float baseDamage = 10f;   // »ù´¡ÉËº¦
    public float attackSpeed = 1.0f; // ¹¥ËÙ

    [Header("Components")]
    public Animator animator;
    public AudioSource audioSource;

    /// <summary>
    /// ¼ÓÉËº¦
    /// </summary>
    public void AddDamage(float amount)
    {
        baseDamage += amount;
        Debug.Log($"[{name}] Damage increased by {amount}, now {baseDamage}");
    }

    /// <summary>
    /// ¼Ó¹¥ËÙ
    /// </summary>
    public void AddAttackSpeed(float amount)
    {
        attackSpeed += amount;
        Debug.Log($"[{name}] Attack speed increased by {amount}, now {attackSpeed}");
    }

    /// <summary>
    /// Æ½aÀäÈ´
    /// </summary>
    public float GetAttackInterval()
    {
        return 1f / attackSpeed;
    }

    /// <summary>
    /// Melee and Ranged
    /// </summary>
    public virtual void Attack()
    {
        Debug.Log($"[{name}] performs {weaponType} attack!");
    }
}
