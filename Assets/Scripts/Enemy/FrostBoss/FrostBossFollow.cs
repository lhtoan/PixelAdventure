using UnityEngine;

public class FrostBossFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;

    [Header("Movement Settings")]
    public float walkSpeed = 1.5f;
    public float runSpeed = 3f;
    public float farDistance = 4f;
    [Header("Detection Settings")]
    public float detectRange = 10f;    // Boss chỉ chạy theo nếu player trong phạm vi này


    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;

    private BossSkillController skillController;
    private BossAttack bossAttack;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        skillController = GetComponent<BossSkillController>();
        bossAttack = GetComponent<BossAttack>();
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        float distFromPlayer = Vector2.Distance(transform.position, target.position);

        // ❌ Nếu player quá xa → boss đứng yên, không follow
        if (distFromPlayer > detectRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetBool("moving", false);
            return;
        }

        // ❌ Khi đang attack thì dừng di chuyển
        if (bossAttack != null && bossAttack.IsAttacking)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetBool("moving", false);
            return;
        }

        HandleMovement();
    }


    private void HandleMovement()
    {
        float dist = Vector2.Distance(transform.position, target.position);
        Vector2 dir = (target.position - transform.position).normalized;

        float moveSpeed = (dist > farDistance) ? runSpeed : walkSpeed;

        rb.linearVelocity = new Vector2(dir.x * moveSpeed, rb.linearVelocity.y);
        anim.SetBool("moving", true);

        if (dir.x != 0)
            sr.flipX = dir.x > 0;
    }

    public bool HasTargetInRange()
    {
        if (target == null) return false;
        float dist = Vector2.Distance(transform.position, target.position);
        return dist <= detectRange;
    }

}
