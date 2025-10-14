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

    private void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>() ?? gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
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
        while (Exp >= Level * 100)
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
}
