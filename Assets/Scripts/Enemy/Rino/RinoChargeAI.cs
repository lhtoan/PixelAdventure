// using UnityEngine;

// public class RinoChargeAI : MonoBehaviour
// {
//     [Header("Detect Area")]
//     public float detectWidth = 3f;
//     public float detectHeight = 1.5f;
//     public float detectOffset = 1f;
//     public LayerMask playerLayer;

//     [Header("Charge Settings")]
//     public float chargeSpeed = 10f;
//     public float cooldown = 2f;
//     public float stunDuration = 2f;

//     private bool isCharging = false;
//     private bool isCooldown = false;

//     private Rigidbody2D rb;
//     private Transform player;

//     private void Awake()
//     {
//         rb = GetComponent<Rigidbody2D>();
//     }

//     private void Update()
//     {
//         if (isCharging || isCooldown)
//             return;

//         if (PlayerInArea())
//             StartCharge();
//     }

//     private bool PlayerInArea()
//     {
//         Vector2 direction = new Vector2(transform.localScale.x, 0f).normalized;
//         Vector2 boxCenter = (Vector2)transform.position + direction * detectOffset;

//         RaycastHit2D hit = Physics2D.BoxCast(
//             boxCenter,
//             new Vector2(detectWidth, detectHeight),
//             0f,
//             Vector2.zero,
//             0f,
//             playerLayer
//         );

//         if (hit.collider != null)
//         {
//             player = hit.collider.transform;
//             return true;
//         }

//         return false;
//     }

//     private void StartCharge()
//     {
//         isCharging = true;

//         Vector2 dir = (player.position - transform.position).normalized;

//         rb.linearVelocity = dir * chargeSpeed;

//         Invoke(nameof(StopCharge), 0.5f);
//     }

//     private void StopCharge()
//     {
//         isCharging = false;
//         rb.linearVelocity = Vector2.zero;

//         isCooldown = true;
//         Invoke(nameof(ResetCooldown), cooldown);
//     }

//     private void ResetCooldown()
//     {
//         isCooldown = false;
//     }

//     private void OnCollisionEnter2D(Collision2D collision)
//     {
//         if (!isCharging) return;

//         if (collision.gameObject.CompareTag("Player"))
//         {
//             StunPlayer(collision.gameObject);
//         }

//         StopCharge();
//     }

//     private void StunPlayer(GameObject playerObj)
//     {
//         Animator anim = playerObj.GetComponent<Animator>();
//         Rigidbody2D prb = playerObj.GetComponent<Rigidbody2D>();

//         if (anim != null)
//         {
//             anim.SetBool("isStun", true);
//             StartCoroutine(UnstunAfterDelay(anim));
//         }

//         if (prb != null)
//         {
//             prb.linearVelocity = Vector2.zero;
//         }
//     }

//     private System.Collections.IEnumerator UnstunAfterDelay(Animator anim)
//     {
//         yield return new WaitForSeconds(stunDuration);
//         anim.SetBool("isStun", false);
//     }

//     private void OnDrawGizmos()
//     {
//         Gizmos.color = Color.red;

//         Vector2 direction =
//             Application.isPlaying
//             ? new Vector2(transform.localScale.x, 0f).normalized
//             : (transform.localScale.x >= 0 ? Vector2.right : Vector2.left);

//         Vector2 boxCenter = (Vector2)transform.position + direction * detectOffset;

//         Gizmos.DrawWireCube(
//             boxCenter,
//             new Vector3(detectWidth, detectHeight, 1f)
//         );
//     }
// }
using UnityEngine;
using System.Collections;

public class RinoPatrolCharge : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform posA;
    public Transform posB;

    [Header("Patrol Settings")]
    public float patrolSpeed = 2f;
    public float idleDuration = 0.5f;

    [Header("Detect Area")]
    public float detectWidth = 3f;
    public float detectHeight = 1.5f;
    public float detectOffset = 1f;
    public LayerMask playerLayer;

    [Header("Charge Settings")]
    public float chargeSpeed = 10f;
    public float maxChargeDistance = 10f;
    public float cooldown = 2f;
    public float stunDuration = 2f;

    private Rigidbody2D rb;
    private Animator anim;

    private bool movingLeft;
    private bool isCharging = false;
    private bool isCooldown = false;
    private bool isPreparingCharge = false;   // ⭐ FIX QUAN TRỌNG – ngăn patrol lật hướng khi charge

    private Vector3 initScale;
    private Transform player;
    private Vector3 chargeStartPos;
    private float idleTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        initScale = transform.localScale;

        movingLeft = posA.position.x < transform.position.x;
    }

    private void Update()
    {
        // ⭐ Ngăn patrol chạy lung tung khi chuẩn bị charge hoặc đang charge
        if (isPreparingCharge || isCharging || isCooldown)
        {
            return;
        }

        if (PlayerInArea())
        {
            StartCharge();
        }
        else
        {
            Patrol();
        }
    }

    // ---------------------- PATROL ----------------------
    private void Patrol()
    {
        float left = Mathf.Min(posA.position.x, posB.position.x);
        float right = Mathf.Max(posA.position.x, posB.position.x);

        if (movingLeft)
        {
            if (transform.position.x > left) Move(-1);
            else ChangeDirection();
        }
        else
        {
            if (transform.position.x < right) Move(1);
            else ChangeDirection();
        }
    }

    private void Move(int dir)
    {
        idleTimer = 0;

        // ⭐ Flip đúng hướng patrol
        transform.localScale = new Vector3(
            Mathf.Abs(initScale.x) * -dir,
            initScale.y,
            initScale.z
        );

        rb.linearVelocity = new Vector2(dir * patrolSpeed, rb.linearVelocity.y);
    }

    private void ChangeDirection()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleDuration)
        {
            movingLeft = !movingLeft;
            idleTimer = 0;

            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    // ---------------------- DETECT ----------------------
    private bool PlayerInArea()
    {
        Vector2 direction = new Vector2(transform.localScale.x, 0).normalized;
        Vector2 boxCenter = (Vector2)transform.position + direction * detectOffset;

        RaycastHit2D hit = Physics2D.BoxCast(
            boxCenter,
            new Vector2(detectWidth, detectHeight),
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

    // ---------------------- CHARGE ----------------------
    private void StartCharge()
    {
        // ⭐ Ngăn patrol RINO flip ngược lại 1 frame tiếp theo
        isPreparingCharge = true;

        StartCoroutine(DelayCharge());
    }

    // ⭐ Delay 1 frame để tránh Patrol làm lật hướng
    private IEnumerator DelayCharge()
    {
        yield return null;

        isPreparingCharge = false;
        isCharging = true;

        // ⭐ Set hướng nhìn theo Player
        float dirX = player.position.x - transform.position.x;
        int look = dirX > 0 ? 1 : -1;

        transform.localScale = new Vector3(
            Mathf.Abs(initScale.x) * look,
            initScale.y,
            initScale.z
        );

        // ⭐ Bắt đầu charge
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * chargeSpeed;

        chargeStartPos = transform.position;
    }

    private void StopCharge()
    {
        isCharging = false;
        rb.linearVelocity = Vector2.zero;

        isCooldown = true;
        Invoke(nameof(ResetCooldown), cooldown);
    }

    private void ResetCooldown()
    {
        isCooldown = false;
    }

    // ---------------------- COLLISION ----------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isCharging) return;

        // Chạm Player → stun → dừng charge
        if (collision.gameObject.CompareTag("Player"))
        {
            StunPlayer(collision.gameObject);
            StopCharge();
            return;
        }

        // ⭐ Ground → Rino luôn đứng trên ground → KHÔNG STOP CHARGE
        if (collision.gameObject.CompareTag("Ground"))
            return;

        // Vật cản / tường
        if (collision.gameObject.CompareTag("Wall"))
        {
            StopCharge();
        }
    }

    private void StunPlayer(GameObject obj)
    {
        Animator a = obj.GetComponent<Animator>();
        Rigidbody2D prb = obj.GetComponent<Rigidbody2D>();

        if (a != null)
        {
            a.SetBool("isStun", true);
            StartCoroutine(Unstun(a));
        }

        if (prb != null)
        {
            prb.linearVelocity = Vector2.zero;
        }
    }

    private IEnumerator Unstun(Animator a)
    {
        yield return new WaitForSeconds(stunDuration);
        a.SetBool("isStun", false);
    }

    // ---------------------- GIZMO ----------------------
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector2 dir = Application.isPlaying ?
            new Vector2(transform.localScale.x, 0).normalized :
            Vector2.right;

        Vector2 boxCenter = (Vector2)transform.position + dir * detectOffset;

        Gizmos.DrawWireCube(boxCenter, new Vector3(detectWidth, detectHeight, 1));
    }
}
