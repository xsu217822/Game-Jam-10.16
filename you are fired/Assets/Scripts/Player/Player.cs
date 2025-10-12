using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class Player : MonoBehaviour
{
    // ===== Fields (Data) =====
    // Stats
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float moveSpeed = 1f;
    private int currentHealth;

    // Movement (optional polish)
    [SerializeField] private float acceleration = 0f;   // 0 = 立即起停
    [SerializeField] private float deceleration = 0f;   // 0 = 立即起停
    private float speedMultiplier = 1f;                 // 运行时叠加
                                                        // —— 输入/朝向 ——
    private Vector2 moveInput;   // 键盘/WASD
    private Vector2 aimDir;      // 鼠标指向的单位向量（世界坐标系）
    private Vector3 aimWorldPos; // 鼠标的世界坐标

    // Components
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Camera followCamera;

    // —— 武器系统（四阶段，全部常驻、自动开火） ——
    // 每个阶段一个槽位：0..3
    //[SerializeField] private WeaponBase[] weaponSlots = new WeaponBase[4];

    // UI
    //[SerializeField] private HealthBar healthBar;

    // Camera follow (optional)
    [SerializeField] private Vector2 cameraDeadZone = Vector2.zero;
    [SerializeField] private float cameraLerp = 0f;

    // Events
    public event System.Action<int, int> OnHealthChanged;
    public event System.Action OnDied;
    public event System.Action OnWeaponChanged;

    private void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        // 鼠标瞄准方向
        if (followCamera == null)
            followCamera = Camera.main;

        if (followCamera != null)
        {
            Vector3 mouseScreen = Mouse.current.position.ReadValue();
            aimWorldPos = followCamera.ScreenToWorldPoint(mouseScreen);
            aimWorldPos.z = transform.position.z;

            Vector2 dir = (aimWorldPos - transform.position);
            aimDir = dir.sqrMagnitude > 0.001f ? dir.normalized : Vector2.right;
        }

        // 动画可选
        //if (animator)
        //{
        //    animator.SetFloat("Speed", rb.linearVelocity.magnitude);
        //    animator.SetFloat("MoveX", aimDir.x);
        //    animator.SetFloat("MoveY", aimDir.y);
        //}
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput.normalized * moveSpeed;
    }

    private void LateUpdate()
    {
        // 相机跟随
        if (followCamera)
        {
            Vector3 camPos = followCamera.transform.position;
            camPos.x = transform.position.x;
            camPos.y = transform.position.y;
            followCamera.transform.position = camPos;
        }
    }

    // 新输入系统：移动
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        // Debug.Log($"OnMove: {moveInput}");
    }

    // 用于别的系统读取当前瞄准方向
    public Vector2 GetAimDir() => aimDir;
    public Vector3 GetAimWorldPos() => aimWorldPos;
}