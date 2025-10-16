using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [Header("Speed")]
    public float moveSpeed = 5f;

    [Header("Audio")]
    [SerializeField] private AudioClip walkingClip;
    [SerializeField, Range(0f,1f)] private float footstepVolume = 0.6f;
    [SerializeField] private float moveThreshold = 0.01f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private AudioSource footstepSource;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        footstepSource = GetComponent<AudioSource>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        // Configure footstep audio
        footstepSource.playOnAwake = false;
        footstepSource.loop = true;
        footstepSource.spatialBlend = 0f; // 2D sound
        footstepSource.volume = footstepVolume;
        footstepSource.clip = walkingClip;
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
        UpdateFootsteps();
    }

    private void UpdateAnimation()
    {
        bool isMoving = moveInput.sqrMagnitude > moveThreshold;

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

    private void UpdateFootsteps()
    {
        if (!walkingClip || !footstepSource) return;

        bool isMoving = moveInput.sqrMagnitude > moveThreshold;

        if (isMoving)
        {
            if (!footstepSource.isPlaying) footstepSource.Play();
        }
        else
        {
            if (footstepSource.isPlaying) footstepSource.Pause();
        }
    }
}
