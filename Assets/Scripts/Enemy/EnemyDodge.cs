using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyDodge : MonoBehaviour
{
    [Header("Dodge Settings")]
    [SerializeField] private float dodgeRange = 3f;
    [SerializeField] private float dodgeHorizontalForce = 6f;
    [SerializeField] private float dodgeVerticalForce = 4f;
    [SerializeField] private float dodgeCooldown = 2f;
    [SerializeField] private LayerMask projectileLayer;

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 8f;          // Lực lao tới player
    [SerializeField] private float dashRange = 4f;          // Khoảng cách kích hoạt dash
    [SerializeField] private float dashCooldown = 3f;       // Thời gian hồi chiêu dash
    [SerializeField] private LayerMask playerLayer;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private float dodgeTimer;
    private float dashTimer;
    private Rigidbody2D rb;
    private Animator anim;
    private bool isDodging;
    private bool isGrounded;
    private Transform player;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (groundCheck == null)
            Debug.LogWarning("⚠️ Chưa gán GroundCheck cho EnemyDodge!");
    }

    private void Update()
    {
        dodgeTimer -= Time.deltaTime;
        dashTimer -= Time.deltaTime;

        CheckGrounded();

        if (!isDodging)
        {
            DetectAndDodge();
            TryDashTowardPlayer();
        }
    }

    private void DetectAndDodge()
    {
        Collider2D projectile = Physics2D.OverlapCircle(transform.position, dodgeRange, projectileLayer);

        if (projectile != null && dodgeTimer <= 0f && isGrounded)
        {
            Vector2 dodgeDir = (transform.position - projectile.transform.position).normalized;
            Vector2 dodgeForce = new Vector2(dodgeDir.x * dodgeHorizontalForce, dodgeVerticalForce);

            rb.linearVelocity = Vector2.zero;
            rb.AddForce(dodgeForce, ForceMode2D.Impulse);

            isDodging = true;
            dodgeTimer = dodgeCooldown;

            Debug.Log($"🦘 Enemy né hướng: {dodgeDir}, lực: {dodgeForce}");
            if (anim && HasParameter(anim, "dodge"))
                anim.SetTrigger("dodge");
        }
    }

    private void TryDashTowardPlayer()
    {
        if (player == null || dashTimer > 0f || !isGrounded) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= dashRange)
        {
            Vector2 dashDir = (player.position - transform.position).normalized;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(dashDir * dashForce, ForceMode2D.Impulse);

            dashTimer = dashCooldown;

            Debug.Log($"⚡ Enemy lao tới player! Hướng: {dashDir}, lực: {dashDir * dashForce}");
            if (anim && HasParameter(anim, "dash"))
                anim.SetTrigger("dash");
        }
    }

    private void CheckGrounded()
    {
        if (groundCheck == null) return;

        Collider2D hit = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        bool wasGrounded = isGrounded;
        isGrounded = hit != null;

        if (isGrounded && !wasGrounded)
            isDodging = false;
    }

    private bool HasParameter(Animator animator, string paramName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
            if (param.name == paramName) return true;
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dodgeRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, dashRange);

        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
