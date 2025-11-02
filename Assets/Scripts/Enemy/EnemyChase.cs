using UnityEngine;

[RequireComponent(typeof(EnemyPatrol))]
public class EnemyChase : MonoBehaviour
{
    [Header("Chase Settings")]
    [SerializeField] private float detectionRange = 5f;          // Phạm vi phát hiện player
    [SerializeField] private float chaseSpeedMultiplier = 1.5f;  // Hệ số tốc độ khi rượt
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Transform detectionPoint;           // Điểm phát hiện
    [SerializeField] private bool showDebug = true;              // Hiển thị gizmo

    private EnemyPatrol patrolScript;
    private Transform player;
    private Animator anim;
    private bool isChasing;
    private float baseSpeed;

    // Giới hạn ranh A-B
    private float leftLimit;
    private float rightLimit;

    private void Awake()
    {
        patrolScript = GetComponent<EnemyPatrol>();
        anim = GetComponentInChildren<Animator>();
        if (detectionPoint == null)
            detectionPoint = transform;

        // Lấy giá trị speed gốc trong EnemyPatrol
        baseSpeed = typeof(EnemyPatrol)
            .GetField("speed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.GetValue(patrolScript) is float s ? s : 2f;

        // Lấy vị trí giới hạn A và B
        leftLimit = patrolScript.leftEdge.position.x;
        rightLimit = patrolScript.rightEdge.position.x;
    }

    private void Update()
    {
        DetectPlayer();

        if (isChasing)
            ChasePlayer();
    }

    private void DetectPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(detectionPoint.position, detectionRange, playerLayer);

        if (hit != null)
        {
            player = hit.transform;

            // Kiểm tra xem player có nằm trong vùng A-B không
            if (player.position.x >= leftLimit && player.position.x <= rightLimit)
            {
                if (!isChasing)
                {
                    isChasing = true;
                    patrolScript.enabled = false;
                    Debug.Log($"👀 Enemy phát hiện player trong khu vực!");
                }
            }
            else
            {
                // Nếu player ra khỏi vùng A-B thì ngừng đuổi
                StopChase();
            }
        }
        else
        {
            // Không thấy player nữa → quay lại tuần tra
            if (isChasing)
                StopChase();
        }
    }

    private void ChasePlayer()
    {
        if (player == null) return;

        float dir = Mathf.Sign(player.position.x - transform.position.x);

        // Nếu player ra khỏi khu vực thì dừng đuổi
        if (player.position.x < leftLimit || player.position.x > rightLimit)
        {
            StopChase();
            return;
        }

        // Xoay hướng enemy
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * dir,
                                           transform.localScale.y,
                                           transform.localScale.z);

        // Di chuyển về phía player (nhưng vẫn trong vùng A-B)
        float targetX = Mathf.Clamp(player.position.x, leftLimit, rightLimit);
        transform.position = Vector2.MoveTowards(
            transform.position,
            new Vector2(targetX, transform.position.y),
            Time.deltaTime * baseSpeed * chaseSpeedMultiplier
        );

        if (anim != null)
            anim.SetBool("moving", true);
    }

    private void StopChase()
    {
        isChasing = false;
        patrolScript.enabled = true;
        player = null;
        Debug.Log("😴 Enemy ngừng rượt, quay lại tuần tra.");
    }

    private void OnDrawGizmosSelected()
    {
        if (!showDebug) return;
        if (detectionPoint == null)
            detectionPoint = transform;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectionPoint.position, detectionRange);

        // Vẽ giới hạn A–B
        if (patrolScript != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(patrolScript.leftEdge.position, patrolScript.rightEdge.position);
        }
    }
}
