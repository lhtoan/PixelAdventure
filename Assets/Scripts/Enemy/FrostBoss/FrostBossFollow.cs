// using UnityEngine;

// public class FrostBossFollow : MonoBehaviour
// {
//     [Header("Target Settings")]
//     public Transform target;

//     [Header("Movement Settings")]
//     public float walkSpeed = 1.5f;
//     public float runSpeed = 3f;
//     public float farDistance = 4f;

//     private Rigidbody2D rb;
//     private Animator anim;
//     private SpriteRenderer sr;
//     private BossSkillController skillController;

//     private void Awake()
//     {
//         rb = GetComponent<Rigidbody2D>();
//         anim = GetComponent<Animator>();
//         sr = GetComponent<SpriteRenderer>();
//         skillController = GetComponent<BossSkillController>(); // <-- QUAN TRỌNG
//     }

//     private void FixedUpdate()
//     {
//         if (target == null) return;

//         // 🔥 Nếu boss đang dùng skill → đứng yên, không chạy movement
//         if (skillController != null && skillController.IsUsingSkill)
//         {
//             rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
//             anim.SetBool("moving", false);
//             return; // <-- QUAN TRỌNG: KHÔNG CHẠY HANDLEMOVEMENT()
//         }

//         HandleMovement();
//     }

//     private void HandleMovement()
//     {
//         float dist = Vector2.Distance(transform.position, target.position);
//         Vector2 dir = (target.position - transform.position).normalized;

//         float moveSpeed = (dist > farDistance) ? runSpeed : walkSpeed;

//         // di chuyển
//         rb.linearVelocity = new Vector2(dir.x * moveSpeed, rb.linearVelocity.y);
//         anim.SetBool("moving", true);

//         // flip sprite
//         if (dir.x != 0)
//             sr.flipX = dir.x > 0;
//     }
// }
using UnityEngine;

public class FrostBossFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;

    [Header("Movement Settings")]
    public float walkSpeed = 1.5f;
    public float runSpeed = 3f;
    public float farDistance = 4f;

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

        // ❌ Không dừng khi dùng Skill1 → chỉ dừng khi Attack
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
}
