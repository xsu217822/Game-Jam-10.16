using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Speed")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput.normalized * moveSpeed;
    }

    private void Update()
    {
        // 检查是否有任何方向输入
        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        // 更新动画参数
        anim.SetBool("isMove", isMoving);
        anim.SetFloat("velocityX", moveInput.x);
        anim.SetFloat("velocityY", moveInput.y);
    }
}
