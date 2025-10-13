using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    private int maxHealth, curHealth;
    private float moveSpeed;
    private Vector2 move;

    public bool IsDead => curHealth <= 0;

    public void ApplyData(PlayerData d)
    {
        if (!rb) rb = GetComponent<Rigidbody2D>() ?? gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        maxHealth = d ? d.maxHealth : 100;
        curHealth = maxHealth;
        moveSpeed = d ? d.moveSpeed : 5f;
    }

    public void OnMove(InputValue v) => move = v.Get<Vector2>();
    private void FixedUpdate() { if (!IsDead) rb.linearVelocity = move.normalized * moveSpeed; }
    public void TakeDamage(int dmg) { curHealth = Mathf.Max(0, curHealth - Mathf.Max(0, dmg)); }
}