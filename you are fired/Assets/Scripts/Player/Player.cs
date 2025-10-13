// Assets/Scripts/Player.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    public int MaxHP = 100, HP = 100;
    public float MoveSpeed = 5f;
    public bool IsDead => HP <= 0;
    private Vector2 move;

    // 新增经验值和等级字段
    public int Exp { get; private set; } = 0;
    public int Level { get; private set; } = 1;

    // 新增经验值累加方法
    public void AddExp(int amount)
    {
        Exp += Mathf.Max(0, amount);
        // 每100经验值升一级
        while (Exp >= Level * 100)
        {
            Exp -= Level * 100;
            LevelUp();
        }
    }

    // 新增升级方法
    private void LevelUp()
    {
        Level++;
        // 可在此处添加升级时的属性提升或特效
        Debug.Log($"玩家升级到 {Level} 级！");
    }

    public void ApplyBase(int hp, float speed)
    {
        if (!rb) rb = GetComponent<Rigidbody2D>() ?? gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        MaxHP = hp; HP = hp; MoveSpeed = speed;
    }

    public void OnMove(InputValue v) => move = v.Get<Vector2>();
    private void FixedUpdate()
    {
        if (!IsDead)
            rb.linearVelocity = move.normalized * MoveSpeed;
        else
            rb.linearVelocity = Vector2.zero;
    }
    public void TakeDamage(int dmg) { HP = Mathf.Max(0, HP - Mathf.Max(0, dmg)); }
}
