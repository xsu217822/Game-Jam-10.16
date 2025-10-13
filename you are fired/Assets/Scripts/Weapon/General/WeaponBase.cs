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
    public float range = 1.5f;       // ��������
    public float baseDamage = 10f;   // �����˺�
    public float attackSpeed = 1.0f; // ����

    [Header("Components")]
    public Animator animator;
    public AudioSource audioSource;

    /// <summary>
    /// ���˺�
    /// </summary>
    public void AddDamage(float amount)
    {
        baseDamage += amount;
        Debug.Log($"[{name}] Damage increased by {amount}, now {baseDamage}");
    }

    /// <summary>
    /// �ӹ���
    /// </summary>
    public void AddAttackSpeed(float amount)
    {
        attackSpeed += amount;
        Debug.Log($"[{name}] Attack speed increased by {amount}, now {attackSpeed}");
    }

    /// <summary>
    /// ƽa��ȴ
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
