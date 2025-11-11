// using UnityEngine;
// using System.Collections.Generic;
// using System.Collections;

// [RequireComponent(typeof(BoxCollider2D))]
// public class HellfireTornado : MonoBehaviour
// {
//     [Header("Tornado Movement")]
//     [SerializeField] private float moveSpeed = 2f;
//     [SerializeField] private float moveDistance = 6f;
//     [SerializeField] private bool useAutoDisable = true;
//     [SerializeField] private float disableDelayAfterStop = 0.3f;

//     [Header("Combat Settings")]
//     [SerializeField] private float instantHitDamage = 10f;  // 💥 Damage ngay lần đầu chạm
//     [SerializeField] private float damagePerSecond = 6f;    // 🔥 Damage liên tục (DOT)

//     private Vector2 moveDir = Vector2.right;
//     private bool active = false;
//     private Vector3 startPosition;
//     private float traveled = 0f;
//     private BoxCollider2D boxCollider;

//     private HashSet<GameObject> burnedEnemies = new HashSet<GameObject>();
//     private HashSet<GameObject> firstHitEnemies = new HashSet<GameObject>(); // 💥 Lưu enemy đã dính hit đầu tiên

//     private void Awake()
//     {
//         boxCollider = GetComponent<BoxCollider2D>();
//         boxCollider.isTrigger = true;
//     }

//     private void OnEnable()
//     {
//         active = true;
//         traveled = 0f;
//         startPosition = transform.position;
//         burnedEnemies.Clear();
//         firstHitEnemies.Clear();
//     }

//     public void Initialize(Vector2 direction)
//     {
//         moveDir = direction.sqrMagnitude > 0.001f ? direction.normalized : Vector2.right;
//     }

//     private void Update()
//     {
//         if (!active) return;

//         Vector3 delta = (Vector3)moveDir * moveSpeed * Time.deltaTime;
//         transform.Translate(delta, Space.World);
//         traveled += delta.magnitude;

//         if (useAutoDisable && traveled >= moveDistance)
//         {
//             StartCoroutine(StopAndDisable());
//             active = false;
//         }
//     }

//     private IEnumerator StopAndDisable()
//     {
//         yield return new WaitForSeconds(disableDelayAfterStop);
//         gameObject.SetActive(false);
//     }

//     private void OnTriggerStay2D(Collider2D collision)
//     {
//         if (!active || !collision.CompareTag("Enemy")) return;

//         var health = collision.GetComponent<Health>();
//         if (health != null)
//         {
//             // 💥 Hit đầu tiên (một lần duy nhất)
//             if (!firstHitEnemies.Contains(collision.gameObject))
//             {
//                 health.TakeDamage(instantHitDamage, true); // bật anim hurt
//                 firstHitEnemies.Add(collision.gameObject);
//                 Debug.Log($"🔥 Tornado first hit {collision.name} for {instantHitDamage} damage!");
//             }

//             // 🔥 Damage liên tục theo thời gian (DOT)
//             health.TakeDamage(damagePerSecond * Time.deltaTime, false);
//         }

//         // 🔥 Gây Burn (1 lần duy nhất)
//         if (!burnedEnemies.Contains(collision.gameObject))
//         {
//             BurnEnemy burn = collision.GetComponent<BurnEnemy>();
//             if (burn != null)
//             {
//                 burn.TriggerBurn();
//                 burnedEnemies.Add(collision.gameObject);
//             }
//         }
//     }
// }

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class HellfireTornado : MonoBehaviour
{
    [Header("Tornado Movement")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float rotateSpeed = 6f;   // tốc độ xoay hướng
    [SerializeField] private float moveDistance = 8f;
    [SerializeField] private bool useAutoDisable = true;
    [SerializeField] private float disableDelayAfterStop = 0.3f;

    [Header("Combat Settings")]
    [SerializeField] private float instantHitDamage = 10f;
    [SerializeField] private float damagePerSecond = 6f;

    private Transform targetEnemy = null;
    private Vector3 startPosition;
    private float traveled = 0f;
    private bool active = false;

    private HashSet<GameObject> burnedEnemies = new HashSet<GameObject>();
    private HashSet<GameObject> firstHitEnemies = new HashSet<GameObject>();

    private void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnEnable()
    {
        active = true;
        traveled = 0f;
        startPosition = transform.position;
        burnedEnemies.Clear();
        firstHitEnemies.Clear();

        // tìm enemy gần nhất khi spawn
        targetEnemy = FindClosestEnemy();
    }

    private void Update()
    {
        if (!active) return;

        // nếu không có enemy → đứng yên hoặc đi thẳng?
        if (targetEnemy == null)
        {
            targetEnemy = FindClosestEnemy();
            if (targetEnemy == null)
                return; // không có enemy nào cả
        }

        // Vector hướng tới enemy
        Vector2 dir = (targetEnemy.position - transform.position).normalized;

        // xoay dần hướng (cho mượt)
        Vector2 newDir = Vector2.Lerp(transform.right, dir, rotateSpeed * Time.deltaTime).normalized;
        transform.right = newDir;

        // di chuyển theo hướng đã xoay
        Vector3 delta = newDir * moveSpeed * Time.deltaTime;
        transform.position += delta;
        traveled += delta.magnitude;

        // hết tầm tồn tại
        if (useAutoDisable && traveled >= moveDistance)
        {
            StartCoroutine(StopAndDisable());
            active = false;
        }
    }

    private Transform FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (var e in enemies)
        {
            float dist = Vector2.Distance(transform.position, e.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = e.transform;
            }
        }

        return closest;
    }

    private IEnumerator StopAndDisable()
    {
        yield return new WaitForSeconds(disableDelayAfterStop);
        gameObject.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!active || !collision.CompareTag("Enemy")) return;

        var health = collision.GetComponent<Health>();
        if (health != null)
        {
            if (!firstHitEnemies.Contains(collision.gameObject))
            {
                health.TakeDamage(instantHitDamage, true);
                firstHitEnemies.Add(collision.gameObject);
            }

            health.TakeDamage(damagePerSecond * Time.deltaTime, false);
        }

        if (!burnedEnemies.Contains(collision.gameObject))
        {
            BurnEnemy burn = collision.GetComponent<BurnEnemy>();
            if (burn != null)
            {
                burn.TriggerBurn();
                burnedEnemies.Add(collision.gameObject);
            }
        }
    }

    public void Initialize(Vector2 dir)
    {
        // Không dùng nữa, nhưng cần để tránh lỗi từ Skill_R_Fire
    }
}
