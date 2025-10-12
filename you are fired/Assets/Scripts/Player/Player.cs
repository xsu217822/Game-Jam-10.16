using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class Player : MonoBehaviour
{
    // ===== Fields (Data) =====
    [Header("Identity")]
    [SerializeField] private string playerName = "Hero";

    [Header("Stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int attackValue = 10;
    [SerializeField] private float moveSpeed = 5f;

    [Header("Progression")]
    [SerializeField] private int level = 1;
    [SerializeField] private int exp = 0;
    [SerializeField] private AnimationCurve expCurve = AnimationCurve.Linear(1, 0, 50, 50);
    // expToNext = baseExp + curve.Evaluate(level) * scale
    [SerializeField] private int baseExpToLevel = 20;
    [SerializeField] private int expScale = 10;

    [Header("Weapon")]
    [SerializeField] private WeaponBase weapon; // ��һ�������� WeaponBase �Ľű�����

    // ===== Components =====
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;      // ���� Sprite �� Animator
    [SerializeField] private Camera followCamera;    // ��ѡ����Ϊ�վͲ�����
    [SerializeField] private HealthBar healthBar;    // UI Ѫ���ű������·���

    // ===== Runtime =====
    private Vector2 moveInput;
    private int currentHealth;

    // �¼������ˡ��������������ⲿϵͳ�ɶ��ģ�
    public event Action<int, int> OnHealthChanged; // (cur,max)
    public event Action OnDied;
    public event Action<int> OnLeveledUp;          // new level

    // ===== Properties��ֻ����¶��=====
    public string PlayerName => playerName;
    public int Level => level;
    public int Exp => exp;
    public int AttackValue => attackValue;
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!animator) animator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;

        // ��ʼ UI
        HealthBarHandler();
    }

    void Start()
    {
        // ��������棨��ѡ��
        if (followCamera != null)
        {
            // ������� z �����������Լ���ͨ�� -10
            followCamera.transform.position = new Vector3(transform.position.x, transform.position.y, followCamera.transform.position.z);
        }
    }

    void Update()
    {
        PlayerAnimation();
        if (followCamera != null)
        {
            var pos = followCamera.transform.position;
            followCamera.transform.position = new Vector3(transform.position.x, transform.position.y, pos.z);
        }
    }

    void FixedUpdate()
    {
        PlayerMovement();
    }

    // ===== Input System��Player Input �󶨵� OnMove / OnFire��=====
    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void OnFire(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (weapon != null) weapon.Fire(this);
    }

    // ===== Functions =====
    public void TakeDamage(int dmg)
    {
        if (dmg <= 0) return;
        currentHealth = Mathf.Max(0, currentHealth - dmg);
        HealthBarHandler();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            // ����
            animator.SetTrigger("Die");
            OnDied?.Invoke();
            enabled = false;        // ��ͣ����
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            animator.SetTrigger("Hurt");
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        HealthBarHandler();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void PlayerMovement()
    {
        // �����ƶ���Top-Down��
        rb.linearVelocity = moveInput.normalized * moveSpeed;
    }

    private void PlayerAnimation()
    {
        // ����������Speed��MoveX��MoveY�����Լ� Animator �Ĳ������ģ�
        float speed = rb ? rb.linearVelocity.magnitude : moveInput.magnitude * moveSpeed;
        animator.SetFloat("Speed", speed);
        if (speed > 0.01f)
        {
            Vector2 dir = rb && rb.linearVelocity.sqrMagnitude > 0.0001f ? rb.linearVelocity.normalized : moveInput.normalized;
            animator.SetFloat("MoveX", dir.x);
            animator.SetFloat("MoveY", dir.y);
        }
    }

    public void AddExp(int amount)
    {
        if (amount <= 0) return;
        exp += amount;
        LevelupHandler();
    }

    private void LevelupHandler()
    {
        int need = baseExpToLevel + Mathf.RoundToInt(expCurve.Evaluate(level) * expScale);
        while (exp >= need)
        {
            exp -= need;
            level++;
            // ����������ʾ����
            maxHealth += 5;
            attackValue += 1;
            moveSpeed += 0.1f;

            currentHealth = maxHealth; // ����
            HealthBarHandler();
            OnLeveledUp?.Invoke(level);

            // ���¼�����һ������
            need = baseExpToLevel + Mathf.RoundToInt(expCurve.Evaluate(level) * expScale);
        }
    }

    private void HealthBarHandler()
    {
        if (healthBar != null)
            healthBar.SetValue(currentHealth, maxHealth);
    }

    // === �������������ṩ�����뿪��ԭ�� ===
    public Vector2 GetAimDirection()
    {
        // ���ָ�򣬻����泯����Jam �򻯣����ƶ����򵱳���
        if (moveInput.sqrMagnitude > 0.0001f) return moveInput.normalized;

        // ��곯����Ļ�����磩
        if (Camera.main != null)
        {
            Vector3 mouse = Mouse.current != null
                ? Mouse.current.position.ReadValue()
                : (Vector3)Input.mousePosition;

            Vector3 world = Camera.main.ScreenToWorldPoint(mouse);
            world.z = transform.position.z;
            Vector2 dir = (world - transform.position);
            if (dir.sqrMagnitude > 0.001f) return dir.normalized;
        }
        return Vector2.right;
    }

    public Vector3 GetMuzzleWorldPos()
    {
        // ������ڽ�ɫ�¹�һ�������� Muzzle�������� Transform �Ͻ���;
        // ����Ϊ�򻯣�ֱ�������λ��
        return transform.position + (Vector3)(GetAimDirection() * 0.3f);
    }
}

/* ================== ֧���ࣨ�Ǳ��裬�����㼴�弴�ã� ================== */

// ��Ѫ�������� UI ����Ļ UI ������
[Serializable]
public class HealthBar : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Slider slider; // ���� Image ������Լ���
    public void SetValue(int cur, int max)
    {
        if (!slider) return;
        slider.minValue = 0;
        slider.maxValue = max;
        slider.value = cur;
    }
}

// �������ࣨ�� Inspector �� Player �� weapon ָ�����������ࣩ
public abstract class WeaponBase : MonoBehaviour
{
    public abstract void Fire(Player owner);
}

// һ����ֱ�Ӳ��Եļ�����������ҳ�����һ���ӵ�
class SimpleGun : WeaponBase
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 12f;
    [SerializeField] private float cooldown = 0.15f;
    private float lastFireTime = -999f;

    public override void Fire(Player owner)
    {
        if (Time.time < lastFireTime + cooldown) return;
        lastFireTime = Time.time;

        if (bulletPrefab == null) return;
        Vector3 pos = owner.GetMuzzleWorldPos();
        Vector2 dir = owner.GetAimDirection();

        var go = Instantiate(bulletPrefab, pos, Quaternion.identity);
        var rb = go.GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = dir * bulletSpeed;
    }
}
