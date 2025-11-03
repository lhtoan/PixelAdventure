using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyDodge : MonoBehaviour
{
    [Header("Dodge Settings")]
    [SerializeField] private float dodgeHorizontalForce = 6f;
    [SerializeField] private float dodgeVerticalForce = 4f;
    [SerializeField] private float dodgeCooldown = 2f;
    [SerializeField] private LayerMask projectileLayer;
    [SerializeField] private float detectRadius = 8f;            // Phạm vi phát hiện đạn
    [SerializeField] private float reactionTime = 0.6f;          // Enemy muốn né trước bao nhiêu giây khi đạn sắp chạm
    [SerializeField] private float maxAngleToReact = 30f;        // Đạn phải bay hướng tới enemy (độ lệch nhỏ hơn giá trị này)

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 8f;
    [SerializeField] private float dashRange = 4f;
    [SerializeField] private float dashCooldown = 3f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private float dodgeTimer;
    private float dashTimer;
    private Rigidbody2D rb;
    private Animator anim;
    private Transform player;
    private bool isGrounded;
    private bool isBusy;

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

        if (!isBusy)
        {
            SmartPredictiveDodge();
            TryDashTowardPlayer();
        }

        DebugStatus();
    }

    // 🧠 Né dự đoán động theo tốc độ đạn
    private void SmartPredictiveDodge()
    {
        if (dodgeTimer > 0f || !isGrounded) return;

        Collider2D[] projectiles = Physics2D.OverlapCircleAll(transform.position, detectRadius, projectileLayer);
        foreach (var proj in projectiles)
        {
            Rigidbody2D prb = proj.GetComponent<Rigidbody2D>();
            if (prb == null) continue;

            Vector2 toEnemy = (Vector2)transform.position - prb.position;
            float distance = toEnemy.magnitude;
            float speed = prb.linearVelocity.magnitude;

            if (speed <= 0.1f) continue;

            // Tính thời gian để đạn chạm enemy
            float timeToHit = distance / speed;

            // Nếu đạn đang bay về phía enemy và sắp tới trong reactionTime giây → né
            float angle = Vector2.Angle(prb.linearVelocity, toEnemy);
            if (angle < maxAngleToReact && timeToHit <= reactionTime)
            {
                Vector2 dodgeDir = prb.linearVelocity.x > 0 ? Vector2.left : Vector2.right;
                Debug.Log(
                    $"🧠 Đạn {proj.name} tốc {speed:F1}, cách {distance:F2}m, chạm sau {timeToHit:F2}s → Né {dodgeDir} (reaction={reactionTime:F1}s)"
                );

                Dodge(dodgeDir);
                break;
            }
        }
    }

    private void Dodge(Vector2 dodgeDir)
    {
        if (isBusy) return;

        Vector2 dodgeForce = new Vector2(dodgeDir.x * dodgeHorizontalForce, dodgeVerticalForce);
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dodgeForce, ForceMode2D.Impulse);

        isBusy = true;
        dodgeTimer = dodgeCooldown;

        if (anim && HasParameter(anim, "dodge"))
            anim.SetTrigger("dodge");

        Debug.Log($"🦘 Enemy né sang {dodgeDir} | Lực: {dodgeForce}");
        Invoke(nameof(ResetBusy), 0.6f);
    }

    private void TryDashTowardPlayer()
    {
        if (player == null || dashTimer > 0f || !isGrounded || isBusy) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= dashRange)
        {
            Vector2 dashDir = (player.position - transform.position).normalized;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(dashDir * dashForce, ForceMode2D.Impulse);

            dashTimer = dashCooldown;
            isBusy = true;

            if (anim && HasParameter(anim, "dash"))
                anim.SetTrigger("dash");

            Debug.Log($"⚡ Enemy dash tới player! Hướng: {dashDir}, lực: {dashDir * dashForce}");
            Invoke(nameof(ResetBusy), 0.8f);
        }
    }

    private void CheckGrounded()
    {
        if (groundCheck == null) return;
        Collider2D hit = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isGrounded = hit != null;
    }

    private void ResetBusy() => isBusy = false;

    private bool HasParameter(Animator animator, string paramName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
            if (param.name == paramName) return true;
        return false;
    }

    private void DebugStatus()
    {
        string state = isBusy ? "BUSY" : "IDLE";
        Debug.Log($"[EnemyDodge] ▶ {state} | Grounded: {isGrounded} | DodgeCD: {dodgeTimer:F2}s | DashCD: {dashTimer:F2}s");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);

        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
