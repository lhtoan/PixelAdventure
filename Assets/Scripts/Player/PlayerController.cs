using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // [SerializeField] private float moveSpeed = 5f;
    // [SerializeField] private float jumpForce = 15f;
    // // [SerializeField] private float dashForce = 20f;
    // [SerializeField] private LayerMask groundLayer;
    // [SerializeField] private Transform groundCheck;

    // private Animator animator;
    // private Rigidbody2D rb;

    // private bool isGrounded;
    // private bool canDoubleJump;
    // // private bool isDashing = false;

    // private void Awake()
    // {
    //     animator = GetComponent<Animator>();
    //     rb = GetComponent<Rigidbody2D>();
    // }

    // private void Update()
    // {
    //     HandleMovement();
    //     HandleJump();

    // }

    // private void HandleMovement()
    // {
    //     float moveInput = Input.GetAxis("Horizontal");
    //     rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

    //     if (moveInput > 0)
    //         transform.localScale = new Vector3(1, 1, 1);
    //     else if (moveInput < 0)
    //         transform.localScale = new Vector3(-1, 1, 1);

    //     animator.SetBool("isRun", Mathf.Abs(rb.linearVelocity.x) > 0.1f);
    // }

    // private void HandleJump()
    // {
    //     if (Input.GetButtonDown("Jump") && isGrounded)
    //     {
    //         rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    //         canDoubleJump = true;
    //     }
    //     else if (canDoubleJump && Input.GetButtonDown("Jump"))
    //     {
    //         rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    //         canDoubleJump = false;
    //     }

    //     isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

    //     bool isJumping = !isGrounded;
    //     animator.SetBool("isJump", isJumping);

    // }

    // public bool canAttack()
    // {
    //     return true;
    // }
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 10f;         // tổng stamina
    [SerializeField] private float staminaRegenRate = 2f;    // tốc độ hồi
    [SerializeField] private float jumpStaminaCost = 2f;     // tốn khi nhảy
    [SerializeField] private float attackStaminaCost = 1f;   // tốn khi tấn công

    private float currentStamina;
    private bool isGrounded;
    private bool canDoubleJump;

    private Animator animator;
    private Rigidbody2D rb;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentStamina = maxStamina;
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
        RegenerateStamina();
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

        if (Input.GetButtonDown("Jump") && isGrounded && HasEnoughStamina(jumpStaminaCost))
        {
            ConsumeStamina(jumpStaminaCost);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            canDoubleJump = true;
        }
        else if (canDoubleJump && Input.GetButtonDown("Jump") && HasEnoughStamina(jumpStaminaCost))
        {
            ConsumeStamina(jumpStaminaCost);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            canDoubleJump = false;
        }

        animator.SetBool("isJump", !isGrounded);
    }

    // --- Hồi stamina dần ---
    private void RegenerateStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
    }

    // --- Kiểm tra khi tấn công ---
    public bool canAttack()
    {
        return HasEnoughStamina(attackStaminaCost);
    }

    public void UseAttackStamina()
    {
        ConsumeStamina(attackStaminaCost);
    }

    // --- Xử lý năng lượng ---
    private bool HasEnoughStamina(float cost)
    {
        return currentStamina >= cost;
    }

    private void ConsumeStamina(float cost)
    {
        currentStamina -= cost;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

    // --- Getter cho UI ---
    public float CurrentMana => currentStamina;
    public float MaxMana => maxStamina;
}
