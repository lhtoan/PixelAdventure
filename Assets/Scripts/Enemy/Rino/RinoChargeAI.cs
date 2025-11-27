using UnityEngine;
using System.Collections;

public class RinoPatrol : MonoBehaviour
{
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

    private int logicFacing = -1;          // sprite gốc nhìn TRÁI
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

        // Hướng patrol ban đầu
        if (posA != null && posB != null)
        {
            float mid = (posA.position.x + posB.position.x) * 0.5f;
            movingLeft = transform.position.x > mid;
            logicFacing = movingLeft ? -1 : 1;
        }
        else
        {
            movingLeft = true;
            logicFacing = -1;
        }

        UpdateVisualFlip();
    }

    private void Update()
    {
        if (isCharging)
        {
            // Đẩy liên tục cho chắc
            rb.linearVelocity = new Vector2(logicFacing * chargeSpeed, rb.linearVelocity.y);

            float dist = Vector2.Distance(transform.position, chargeStartPos);
            //Debug.Log($"[RINO] CHARGING | posX = {transform.position.x:F2}, startX = {chargeStartPos.x:F2}, dist = {dist:F2}, max = {maxChargeDistance}");

            // Check hit player trong lúc charge
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

        if (isCooldown)
            return;

        if (DetectPlayer())
            StartCharge();
        else
            Patrol();
    }

    // =============== PATROL =================
    private void Patrol()
    {
        if (posA == null || posB == null) return;

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
            idleTimer = 0;
            logicFacing = movingLeft ? -1 : 1;
            UpdateVisualFlip();
        }
    }

    // =============== DETECT (GIỐNG MELEE) =================
    private bool DetectPlayer()
    {
        Vector2 dir = new Vector2(logicFacing, 0);
        Vector2 boxCenter =
            (Vector2)transform.position + dir * Mathf.Abs(detectOffset);

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

    // =============== CHARGE =================
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

    // =============== CHECK HIT PLAYER WHILE CHARGING =================
    // private bool CheckHitPlayerWhileCharging()
    // {
    //     Vector2 dir = new Vector2(logicFacing, 0);
    //     // Box nhỏ hơn, ở ngay trước mỏ
    //     Vector2 center =
    //         (Vector2)transform.position + dir * 0.5f;

    //     RaycastHit2D hit = Physics2D.BoxCast(
    //         center,
    //         new Vector2(0.8f, detectHeight),
    //         0f,
    //         Vector2.zero,
    //         0f,
    //         playerLayer
    //     );

    //     if (hit.collider != null)
    //     {
    //         // Stun player
    //         Animator pa = hit.collider.GetComponent<Animator>();
    //         if (pa != null)
    //         {
    //             pa.SetBool("isStun", true);
    //             StartCoroutine(Unstun(pa));
    //         }
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
            // ⭐ chỉ stun 1 lần
            PlayerStatus status = hit.collider.GetComponentInParent<PlayerStatus>();
            if (status != null && !status.isStunned)
            {
                status.ApplyStun(stunDuration);
            }

            return true; // StopCharge() sẽ xử lý
        }

        return false;
    }



    // private IEnumerator Unstun(Animator pa)
    // {
    //     yield return new WaitForSeconds(stunDuration);
    //     pa.SetBool("isStun", false);
    // }

    // =============== VISUAL =================
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
