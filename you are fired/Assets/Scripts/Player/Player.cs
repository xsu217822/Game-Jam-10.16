// Assets/Scripts/Player.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField] private Rigidbody2D rb;

    [Header("HP")]
    public int MaxHP = 100;
    public int HP = 100;
    public bool IsDead => HP <= 0;

    [Tooltip("InvincibleSeconds")]
    [SerializeField] private float hitInvincibleSeconds = 0.15f;
    private float lastHitTime = -999f;

    [Header("Move")]
    public float MoveSpeed = 5f;
    private Vector2 move;

    // ���� & �ȼ�
    public int Exp { get; private set; } = 0;
    public int Level { get; private set; } = 1;

    // ��¶�ٷֱȸ�Ѫ�� UI ʹ��
    public float HpPercent => MaxHP <= 0 ? 0f : (float)HP / MaxHP;

    [Header("Weapon Prefabs")]
    public GameObject keyboardPrefab;
    public GameObject injectorPrefab;
    public GameObject gunPrefab;
    public GameObject bottlePrefab;
    public GameObject cleaverPrefab;
    public GameObject penPrefab;
    public GameObject scalpelPrefab;
    public GameObject swordPrefab;

    private WeaponBase equippedWeapon;
    private int weaponIndex = 0;
    private GameObject[] weaponPrefabs;

    private void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>() ?? gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        // Store all weapon prefabs in an array for easy access
        weaponPrefabs = new GameObject[]
        {
            keyboardPrefab,    // 0
            injectorPrefab,    // 1
            gunPrefab,         // 2
            bottlePrefab,      // 3
            cleaverPrefab,     // 4
            penPrefab,         // 5
            scalpelPrefab,     // 6
            swordPrefab        // 7
        };

        // Equip the first weapon by default
        EquipWeapon(weaponPrefabs[weaponIndex]);
    }

    public void ApplyBase(int hp, float speed)
    {
        MaxHP = Mathf.Max(1, hp);
        HP = MaxHP;
        MoveSpeed = speed;
    }

    public void OnMove(InputValue v) => move = v.Get<Vector2>();

    private void FixedUpdate()
    {
        if (!IsDead)
            rb.linearVelocity = move.normalized * MoveSpeed;
        else
            rb.linearVelocity = Vector2.zero;
    }

    public void OnAttack(InputValue value)
    {
        if (equippedWeapon != null)
        {
            equippedWeapon.Attack();
        }
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        if (Time.time < lastHitTime + hitInvincibleSeconds) return;
        lastHitTime = Time.time;

        int dmg = Mathf.CeilToInt(Mathf.Max(0f, amount));
        if (dmg <= 0) return;

        HP = Mathf.Max(0, HP - dmg);

        // TODO: �ܻ�����������/����/�ܻ�����/��Ч
        // animator?.SetTrigger("Hit");

        if (IsDead)
        {
            Die();
        }
    }

    private void Die()
    {
        // TODO: �ر����롢��������������������ײ��֪ͨ GameManager����������/�����
        Debug.Log("[Player] Died");
        // ���磺GetComponent<Collider2D>().enabled = false;
        // animator?.SetBool("Dead", true);
    }

    // ����ϵͳ
    public void AddExp(int amount)
    {       
        Exp += Mathf.Max(0, amount);
        while (Exp >= Level * 100) Exp -= Level * 100;
        {
            // 100 ��������     
            Exp -= Level * 100;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        Level++;
        Debug.Log($"��������� {Level} ����");
    }

    public void EquipWeapon(GameObject weaponPrefab)
    {
        if (equippedWeapon != null)
            Destroy(equippedWeapon.gameObject);

        equippedWeapon = Instantiate(weaponPrefab, transform).GetComponent<WeaponBase>();
        equippedWeapon.transform.localPosition = Vector3.zero;
    }

    // Example: Switch weapon by index (call this from input/UI)
    public void SwitchWeapon(int index)
    {
        if (index < 0 || index >= weaponPrefabs.Length) return;
        weaponIndex = index;
        EquipWeapon(weaponPrefabs[weaponIndex]);
    }

    // Example: Cycle to next weapon (call this from input/UI)
    public void NextWeapon()
    {
        weaponIndex = (weaponIndex + 1) % weaponPrefabs.Length;
        EquipWeapon(weaponPrefabs[weaponIndex]);
    }
}
