using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Speed")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

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
        UpdateAnimation();
        FlipSprite();
    }

    private void UpdateAnimation()
    {
        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        anim.SetBool("isMove", isMoving);
        anim.SetFloat("velocityX", moveInput.x);
        anim.SetFloat("velocityY", moveInput.y);
    }

    private void FlipSprite()
    {
        if (moveInput.x > 0.01f)
        {
            sprite.flipX = false;
        }
        else if (moveInput.x < -0.01f)
        {
            sprite.flipX = true;
        }
    }
}
