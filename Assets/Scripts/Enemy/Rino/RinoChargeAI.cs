using UnityEngine;
using System.Collections;

public class RinoPatrol : MonoBehaviour
{
    [Header("Charge Damage")]
    public float chargeDamage = 1f;

    [Header("Behavior Mode")]
    public bool usePatrol = true;   // TRUE = Patrol A-B | FALSE = Follow target

    [Header("Target (for follow mode)")]
    public Transform target;

    [Header("Follow Settings")]
    public float followRange = 10f;
    public float stopDistance = 0.3f;

    [Header("Patrol Points")]
    public Transform posA;
    public Transform posB;

    [Header("Patrol Settings")]
    public float speed = 2f;
    public float idleDuration = 0.5f;

    [Header("Detect Settings")]
    public float detectRange = 3f;
    public float detectHeight = 1.2f;
    public float detectOffset = 1f;
    public LayerMask playerLayer;

    [Header("Charge Settings")]
    public float chargeSpeed = 14f;
    public float maxChargeDistance = 10f;
    public float cooldown = 2f;
    public float stunDuration = 2f;

    private bool movingLeft = true;
    private bool isCharging = false;
    private bool isCooldown = false;

    private int logicFacing = -1;
    private float idleTimer = 0f;
    private Vector2 chargeStartPos;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private Transform player;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        // Setup facing when patrol is enabled
        if (usePatrol && posA != null && posB != null)
        {
            float mid = (posA.position.x + posB.position.x) * 0.5f;
            movingLeft = transform.position.x > mid;
            logicFacing = movingLeft ? -1 : 1;
        }
        else
        {
            logicFacing = -1;
        }

        UpdateVisualFlip();
    }

    private void Update()
    {
        // ---------------- CHARGING ----------------
        if (isCharging)
        {
            rb.linearVelocity = new Vector2(logicFacing * chargeSpeed, rb.linearVelocity.y);

            float dist = Vector2.Distance(transform.position, chargeStartPos);

            if (CheckHitPlayerWhileCharging())
            {
                StopCharge();
                return;
            }

            if (dist > maxChargeDistance)
            {
                StopCharge();
            }

            return;
        }

        // ---------------- COOLDOWN ----------------
        if (isCooldown)
            return;

        // ---------------- DETECT → CHARGE ----------------
        if (DetectPlayer())
        {
            StartCharge();
            return;
        }

        // ---------------- BEHAVIOR MODE ----------------
        if (usePatrol)
            Patrol();
        else
            FollowTarget();
    }

    // ===============================================================
    // FOLLOW MODE — giống UniversalEnemyMovement
    // ===============================================================
    private void FollowTarget()
    {
        if (target == null)
        {
            anim.SetBool("run", false);
            return;
        }

        float distance = Vector2.Distance(transform.position, target.position);

        // Nếu target quá xa → không follow
        if (distance > followRange)
        {
            anim.SetBool("run", false);
            return;
        }

        // Nếu quá gần → dừng
        if (distance < stopDistance)
        {
            anim.SetBool("run", false);
            return;
        }

        float dx = target.position.x - transform.position.x;

        // Flip giống universal
        logicFacing = dx >= 0 ? 1 : -1;
        UpdateVisualFlip();

        anim.SetBool("run", true);

        // Di chuyển như ground enemy universal
        transform.position += new Vector3(logicFacing * speed * Time.deltaTime, 0, 0);
    }

    // ===============================================================
    // PATROL MODE
    // ===============================================================
    private void Patrol()
    {
        if (posA == null || posB == null)
        {
            anim.SetBool("run", false);
            return;
        }

        float left = Mathf.Min(posA.position.x, posB.position.x);
        float right = Mathf.Max(posA.position.x, posB.position.x);

        if (movingLeft)
        {
            if (transform.position.x > left)
                Move(-1);
            else
                WaitAndTurn();
        }
        else
        {
            if (transform.position.x < right)
                Move(1);
            else
                WaitAndTurn();
        }
    }

    private void Move(int dir)
    {
        idleTimer = 0f;
        logicFacing = dir;
        UpdateVisualFlip();

        anim.SetBool("run", true);
        transform.position += new Vector3(dir * speed * Time.deltaTime, 0, 0);
    }

    private void WaitAndTurn()
    {
        anim.SetBool("run", false);

        idleTimer += Time.deltaTime;
        if (idleTimer >= idleDuration)
        {
            movingLeft = !movingLeft;
            idleTimer = 0f;

            logicFacing = movingLeft ? -1 : 1;
            UpdateVisualFlip();
        }
    }

    // ===============================================================
    // DETECT
    // ===============================================================
    private bool DetectPlayer()
    {
        Vector2 dir = new Vector2(logicFacing, 0);
        Vector2 boxCenter = (Vector2)transform.position + dir * Mathf.Abs(detectOffset);

        RaycastHit2D hit = Physics2D.BoxCast(
            boxCenter,
            new Vector2(detectRange, detectHeight),
            0f,
            Vector2.zero,
            0f,
            playerLayer
        );

        if (hit.collider != null)
        {
            player = hit.collider.transform;
            return true;
        }

        return false;
    }

    // ===============================================================
    // CHARGE
    // ===============================================================
    private void StartCharge()
    {
        isCharging = true;

        float dx = player.position.x - transform.position.x;
        logicFacing = dx > 0 ? 1 : -1;
        UpdateVisualFlip();

        chargeStartPos = transform.position;

        anim.SetBool("run", true);
        rb.linearVelocity = new Vector2(logicFacing * chargeSpeed, rb.linearVelocity.y);
    }

    private void StopCharge()
    {
        isCharging = false;
        rb.linearVelocity = Vector2.zero;

        anim.SetBool("run", false);

        isCooldown = true;
        Invoke(nameof(ResetCooldown), cooldown);
    }

    private void ResetCooldown()
    {
        isCooldown = false;
    }

    // private bool CheckHitPlayerWhileCharging()
    // {
    //     Vector2 dir = new Vector2(logicFacing, 0);
    //     Vector2 center = (Vector2)transform.position + dir * 1.0f;

    //     RaycastHit2D hit = Physics2D.BoxCast(
    //         center,
    //         new Vector2(1.2f, detectHeight),
    //         0f,
    //         Vector2.zero,
    //         0f,
    //         playerLayer
    //     );

    //     if (hit.collider != null)
    //     {
    //         PlayerStatus status = hit.collider.GetComponentInParent<PlayerStatus>();

    //         if (status != null && !status.isStunned)
    //             status.ApplyStun(stunDuration);

    //         return true;
    //     }

    //     return false;
    // }
    private bool CheckHitPlayerWhileCharging()
    {
        Vector2 dir = new Vector2(logicFacing, 0);
        Vector2 center = (Vector2)transform.position + dir * 1.0f;

        RaycastHit2D hit = Physics2D.BoxCast(
            center,
            new Vector2(1.2f, detectHeight),
            0f,
            Vector2.zero,
            0f,
            playerLayer
        );

        if (hit.collider != null)
        {
            // ⭐ Gây damage
            Health hp = hit.collider.GetComponentInParent<Health>();
            if (hp != null)
                hp.TakeDamage(chargeDamage, false); // không bật animation hurt

            // ⭐ Stun nếu có PlayerStatus
            PlayerStatus status = hit.collider.GetComponentInParent<PlayerStatus>();
            if (status != null)
                status.ApplyStun(stunDuration);

            // ⭐ Ngừng di chuyển
            isCharging = false;
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("run", false);

            // ⭐ Bắt đầu quá trình biến mất sau delay
            StartCoroutine(DestroyAfterDelay(0.25f)); // 0.25 giây rồi biến mất

            return true;
        }

        return false;
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }



    private void UpdateVisualFlip()
    {
        if (sprite != null)
            sprite.flipX = (logicFacing == 1);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        int dir = Application.isPlaying ? logicFacing : -1;
        Vector2 center = (Vector2)transform.position + new Vector2(dir * Mathf.Abs(detectOffset), 0);
        Gizmos.DrawWireCube(center, new Vector3(detectRange, detectHeight, 1));
    }
}
