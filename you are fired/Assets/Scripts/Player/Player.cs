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
    [SerializeField] private WeaponBase weapon; // 拖一个派生自 WeaponBase 的脚本上来

    // ===== Components =====
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;      // 挂在 Sprite 的 Animator
    [SerializeField] private Camera followCamera;    // 可选，若为空就不处理
    [SerializeField] private HealthBar healthBar;    // UI 血条脚本（见下方）

    // ===== Runtime =====
    private Vector2 moveInput;
    private int currentHealth;

    // 事件：受伤、死亡、升级（外部系统可订阅）
    public event Action<int, int> OnHealthChanged; // (cur,max)
    public event Action OnDied;
    public event Action<int> OnLeveledUp;          // new level

    // ===== Properties（只读暴露）=====
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

        // 初始 UI
        HealthBarHandler();
    }

    void Start()
    {
        // 简单相机跟随（可选）
        if (followCamera != null)
        {
            // 把相机的 z 留给开发者自己：通常 -10
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

    // ===== Input System（Player Input 绑定到 OnMove / OnFire）=====
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
            // 死亡
            animator.SetTrigger("Die");
            OnDied?.Invoke();
            enabled = false;        // 暂停控制
            rb.velocity = Vector2.zero;
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
        // 物理移动（Top-Down）
        rb.velocity = moveInput.normalized * moveSpeed;
    }

    private void PlayerAnimation()
    {
        // 动画参数：Speed、MoveX、MoveY（按自己 Animator 的参数名改）
        float speed = rb ? rb.velocity.magnitude : moveInput.magnitude * moveSpeed;
        animator.SetFloat("Speed", speed);
        if (speed > 0.01f)
        {
            Vector2 dir = rb && rb.velocity.sqrMagnitude > 0.0001f ? rb.velocity.normalized : moveInput.normalized;
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
            // 升级奖励（示例）
            maxHealth += 5;
            attackValue += 1;
            moveSpeed += 0.1f;

            currentHealth = maxHealth; // 回满
            HealthBarHandler();
            OnLeveledUp?.Invoke(level);

            // 重新计算下一段需求
            need = baseExpToLevel + Mathf.RoundToInt(expCurve.Evaluate(level) * expScale);
        }
    }

    private void HealthBarHandler()
    {
        if (healthBar != null)
            healthBar.SetValue(currentHealth, maxHealth);
    }

    // === 与武器交互：提供朝向与开火原点 ===
    public Vector2 GetAimDirection()
    {
        // 鼠标指向，或用面朝方向。Jam 简化：用移动方向当朝向
        if (moveInput.sqrMagnitude > 0.0001f) return moveInput.normalized;

        // 鼠标朝向（屏幕→世界）
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
        // 你可以在角色下挂一个空物体 Muzzle，把它的 Transform 拖进来;
        // 这里为简化，直接用玩家位置
        return transform.position + (Vector3)(GetAimDirection() * 0.3f);
    }
}

/* ================== 支持类（非必需，但给你即插即用） ================== */

// 简单血条：世界 UI 或屏幕 UI 都可用
[Serializable]
public class HealthBar : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Slider slider; // 若用 Image 做填充自己改
    public void SetValue(int cur, int max)
    {
        if (!slider) return;
        slider.minValue = 0;
        slider.maxValue = max;
        slider.value = cur;
    }
}

// 武器基类（在 Inspector 给 Player 的 weapon 指定具体派生类）
public abstract class WeaponBase : MonoBehaviour
{
    public abstract void Fire(Player owner);
}

// 一个可直接测试的简单武器：朝玩家朝向发射一颗子弹
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
        if (rb) rb.velocity = dir * bulletSpeed;
    }
}
