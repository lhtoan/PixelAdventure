// using UnityEngine;

// public class PlayerController : MonoBehaviour
// {
//     [SerializeField] private float moveSpeed = 5f;
//     [SerializeField] private float jumpForce = 15f;
//     [SerializeField] private LayerMask groundLayer;
//     [SerializeField] private Transform groundCheck;
//     private bool isGrounded;
//     private Rigidbody2D rb;

//     private void Awake()
//     {
//         rb = GetComponent<Rigidbody2D>();
//     }
//     void Start()
//     {

//     }

//     // Update is called once per frame
//     void Update()
//     {
//         HandleMovement();
//         HandleJump();
//     }

//     private void HandleMovement()
//     {
//         float moveInput = Input.GetAxis("Horizontal");
//         rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

//         if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
//         else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
//     }

//     private void HandleJump()
//     {
//         if (Input.GetButton("Jump") && isGrounded)
//         {
//             rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
//         }
//         isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
//     }
// }

// using UnityEngine;

// public class PlayerController : MonoBehaviour
// {
//     [Header("Movement")]
//     [SerializeField] private float moveSpeed = 5f;

//     [Header("Jump")]
//     [SerializeField] private float jumpForce = 10f;
//     [SerializeField] private int maxJumps = 1; // số lần nhảy tối đa (1 = nhảy thường, 2 = double jump)
//     [SerializeField] private LayerMask groundLayer;
//     [SerializeField] private Transform groundCheck;
//     private int jumpCount;
//     private bool isGrounded;

//     [Header("Dash")]
//     [SerializeField] private float dashForce = 20f;
//     [SerializeField] private float dashDuration = 0.2f;
//     [SerializeField] private float dashCooldown = 1f;
//     private bool isDashing;
//     private bool canDash = true;

//     private Rigidbody2D rb;

//     private void Awake()
//     {
//         rb = GetComponent<Rigidbody2D>();
//     }

//     void Update()
//     {
//         HandleMovement();

//         if (Input.GetButtonDown("Jump"))
//         {
//             HandleJump();
//         }

//         if (Input.GetKeyDown(KeyCode.LeftShift) && canDash) // Nhấn Shift để Dash
//         {
//             StartCoroutine(Dash());
//         }
//     }

//     private void HandleMovement()
//     {
//         if (isDashing) return; // khi đang dash thì không điều khiển

//         float moveInput = Input.GetAxis("Horizontal");
//         rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

//         if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
//         else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);

//         // Check ground
//         isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
//         if (isGrounded) jumpCount = 0; // reset khi chạm đất
//     }

//     private void HandleJump()
//     {
//         if (jumpCount < maxJumps)
//         {
//             rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
//             jumpCount++;
//         }
//     }

//     private System.Collections.IEnumerator Dash()
//     {
//         isDashing = true;
//         canDash = false;

//         float originalGravity = rb.gravityScale;
//         rb.gravityScale = 0; // khi dash thì không rơi

//         float dashDirection = transform.localScale.x; // hướng theo facing
//         rb.linearVelocity = new Vector2(dashDirection * dashForce, 0f);

//         yield return new WaitForSeconds(dashDuration);

//         rb.gravityScale = originalGravity;
//         isDashing = false;

//         yield return new WaitForSeconds(dashCooldown);
//         canDash = true;
//     }
// }
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 15f;
    // [SerializeField] private float dashForce = 20f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    private Animator animator;
    private Rigidbody2D rb;

    private bool isGrounded;
    private bool canDoubleJump;
    // private bool isDashing = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
        // UpdateAnimation();
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            canDoubleJump = true;
        }
        else if (canDoubleJump && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            canDoubleJump = false;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    // private void UpdateAnimation()
    // {
    //     bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
    //     bool isJumping = !isGrounded;

    //     animator.SetBool("isRunning", isRunning);
    //     animator.SetBool("isJumping", isJumping);
    // }
}
