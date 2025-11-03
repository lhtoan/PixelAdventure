using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    [Header("Jump Settings")]
    [SerializeField] private float jumpStaminaCost = 2f;

    private bool isGrounded;
    private bool canDoubleJump;

    private Animator animator;
    private Rigidbody2D rb;
    private PlayerStamina stamina;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        stamina = GetComponent<PlayerStamina>();
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        animator.SetBool("isRun", Mathf.Abs(rb.linearVelocity.x) > 0.1f);
    }

    private void HandleJump()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        if (Input.GetButtonDown("Jump") && isGrounded && stamina.CanUse(jumpStaminaCost))
        {
            stamina.Use(jumpStaminaCost);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            canDoubleJump = true;
        }
        else if (canDoubleJump && Input.GetButtonDown("Jump") && stamina.CanUse(jumpStaminaCost))
        {
            stamina.Use(jumpStaminaCost);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            canDoubleJump = false;
        }

        animator.SetBool("isJump", !isGrounded);
    }
}
