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

    // ��������ֵ�͵ȼ��ֶ�
    public int Exp { get; private set; } = 0;
    public int Level { get; private set; } = 1;

    // ��������ֵ�ۼӷ���
    public void AddExp(int amount)
    {
        Exp += Mathf.Max(0, amount);
        // ÿ100����ֵ��һ��
        while (Exp >= Level * 100)
        {
            Exp -= Level * 100;
            LevelUp();
        }
    }

    // ������������
    private void LevelUp()
    {
        Level++;
        // ���ڴ˴��������ʱ��������������Ч
        Debug.Log($"��������� {Level} ����");
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
