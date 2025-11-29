// using UnityEngine;
// using System.Collections.Generic;
// using System.Collections;

// [RequireComponent(typeof(CircleCollider2D))]
// public class FireChasingOrb : MonoBehaviour
// {
//     [Header("Orb Movement")]
//     [SerializeField] private float moveSpeed = 2.5f;
//     [SerializeField] private float rotateSpeed = 6f;
//     [SerializeField] private float moveDistance = 8f;
//     [SerializeField] private bool useAutoDisable = true;
//     [SerializeField] private float disableDelayAfterStop = 0.3f;

//     [Header("Combat Settings")]
//     [SerializeField] private float instantHitDamage = 10f;
//     [SerializeField] private float damagePerSecond = 6f;

//     private Transform targetEnemy = null;
//     private Vector2 moveDirection = Vector2.right;

//     private float traveled = 0f;
//     private bool active = false;
//     private bool isLockedOn = false;

//     private HashSet<GameObject> burnedEnemies = new HashSet<GameObject>();
//     private HashSet<GameObject> firstHitEnemies = new HashSet<GameObject>();

//     private void Awake()
//     {
//         CircleCollider2D col = GetComponent<CircleCollider2D>();
//         col.isTrigger = true;
//     }

//     private void OnEnable()
//     {
//         active = true;
//         traveled = 0f;

//         burnedEnemies.Clear();
//         firstHitEnemies.Clear();

//         isLockedOn = false;

//         moveDirection = Vector2.right;
//         targetEnemy = FindClosestEnemy();
//     }

//     private void Update()
//     {
//         if (!active)
//             return;

//         if (!isLockedOn)
//         {
//             if (targetEnemy == null || !targetEnemy.gameObject.activeInHierarchy)
//                 targetEnemy = FindClosestEnemy();
//         }

//         if (targetEnemy == null)
//             return;

//         if (!isLockedOn)
//         {
//             Vector2 dir = (targetEnemy.position - transform.position).normalized;
//             moveDirection = Vector2.Lerp(moveDirection, dir, rotateSpeed * Time.deltaTime).normalized;
//         }

//         Vector3 delta = (Vector3)moveDirection * moveSpeed * Time.deltaTime;
//         transform.position += delta;
//         traveled += delta.magnitude;

//         if (useAutoDisable && traveled >= moveDistance)
//         {
//             StartCoroutine(StopAndDisable());
//             active = false;
//         }
//     }

//     private void OnTriggerEnter2D(Collider2D collision)
//     {
//         if (collision.CompareTag("Enemy"))
//             isLockedOn = true;
//     }

//     private void OnTriggerExit2D(Collider2D collision)
//     {
//         if (collision.CompareTag("Enemy"))
//             isLockedOn = false;
//     }

//     private Transform FindClosestEnemy()
//     {
//         GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

//         Transform closest = null;
//         float minDist = Mathf.Infinity;

//         foreach (var e in enemies)
//         {
//             if (!e.activeInHierarchy)
//                 continue;

//             float dist = Vector2.Distance(transform.position, e.transform.position);

//             if (dist < minDist)
//             {
//                 minDist = dist;
//                 closest = e.transform;
//             }
//         }

//         return closest;
//     }

//     private IEnumerator StopAndDisable()
//     {
//         yield return new WaitForSeconds(disableDelayAfterStop);
//         gameObject.SetActive(false);
//     }

//     private void OnTriggerStay2D(Collider2D collision)
//     {
//         if (!active || !collision.CompareTag("Enemy"))
//             return;

//         var health = collision.GetComponent<Health>();

//         if (health != null)
//         {
//             if (!firstHitEnemies.Contains(collision.gameObject))
//             {
//                 health.TakeDamage(instantHitDamage, true);
//                 firstHitEnemies.Add(collision.gameObject);
//             }

//             health.TakeDamage(damagePerSecond * Time.deltaTime, false);
//         }

//         // ⭐ LẤY attacker từ Player
//         PlayerAttack attacker = GetComponentInParent<PlayerAttack>();

//         // ⭐ Burn effect chỉ 1 lần mỗi enemy
//         if (!burnedEnemies.Contains(collision.gameObject))
//         {
//             BurnEnemy burn = collision.GetComponent<BurnEnemy>();
//             if (burn != null)
//             {
//                 burn.TriggerBurn(attacker);   // <-- Truyền attacker vào đây để áp DOT BUFF
//                 burnedEnemies.Add(collision.gameObject);
//             }
//         }
//     }

//     public void Initialize(Vector2 dir) { }
// }
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CircleCollider2D))]
public class FireChasingOrb : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float rotateSpeed = 8f;
    public float lifeTime = 10f;

    [Header("Combat")]
    public float instantHitDamage = 10f;
    public float damagePerSecond = 6f;

    private float lifeTimer = 0f;
    private Transform target;
    private Vector2 moveDirection = Vector2.right;

    private bool isLockedOn = false;

    private List<Collider2D> enemiesInRange = new List<Collider2D>();
    private HashSet<GameObject> firstHit = new HashSet<GameObject>();
    private HashSet<GameObject> burned = new HashSet<GameObject>();


    private void Awake()
    {
        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    private void OnEnable()
    {
        lifeTimer = 0f;

        enemiesInRange.Clear();
        firstHit.Clear();
        burned.Clear();

        isLockedOn = false;

        if (moveDirection == Vector2.zero)
            moveDirection = Vector2.right;

        target = FindClosestEnemy();
    }

    private void Update()
    {
        // ====== LIFETIME ======
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifeTime)
        {
            gameObject.SetActive(false);
            return;
        }

        // ====== UPDATE TARGET ======
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            target = FindClosestEnemy();
            isLockedOn = false;
        }

        if (target != null)
        {
            Vector2 dir = (target.position - transform.position).normalized;

            if (isLockedOn)
                moveDirection = dir;  // Bám chặt hướng enemy
            else
                moveDirection = Vector2.Lerp(moveDirection, dir, rotateSpeed * Time.deltaTime);
        }

        // ====== MOVE ======
        transform.position += (Vector3)(moveDirection * moveSpeed * Time.deltaTime);

        // ====== DAMAGE ======
        DealDamageToEnemies();
    }

    // ======================================================
    //                  FIND CLOSEST ENEMY
    // ======================================================
    private Transform FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (var e in enemies)
        {
            if (!e.activeInHierarchy) continue;

            float dist = Vector2.Distance(transform.position, e.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = e.transform;
            }
        }

        return closest;
    }

    // ======================================================
    //                      DAMAGE LOGIC
    // ======================================================
    private void DealDamageToEnemies()
    {
        PlayerAttack atk = GetComponentInParent<PlayerAttack>();

        foreach (var col in enemiesInRange.ToArray())
        {
            if (col == null || !col.gameObject.activeInHierarchy)
            {
                enemiesInRange.Remove(col);
                continue;
            }

            Health hp = col.GetComponent<Health>();
            if (hp != null)
            {
                // Nếu enemy chết → bỏ khỏi danh sách
                if (hp.currentHealth <= 0)
                {
                    enemiesInRange.Remove(col);

                    // Nếu target là enemy này → tìm target mới
                    if (target == col.transform)
                    {
                        target = FindClosestEnemy();
                        isLockedOn = false;
                    }

                    continue;
                }

                // Damage lần đầu
                if (!firstHit.Contains(col.gameObject))
                {
                    hp.TakeDamage(instantHitDamage, true);
                    firstHit.Add(col.gameObject);
                }

                // Damage theo thời gian
                hp.TakeDamage(damagePerSecond * Time.deltaTime, false);
            }

            // Burn 1 lần
            if (!burned.Contains(col.gameObject))
            {
                BurnEnemy burn = col.GetComponent<BurnEnemy>();
                if (burn != null)
                    burn.TriggerBurn(atk);

                burned.Add(col.gameObject);
            }
        }
    }

    // ======================================================
    //                    ENTER – EXIT COLLIDER
    // ======================================================
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        // Lock-on vào enemy đầu tiên trong vùng
        if (!isLockedOn)
        {
            target = other.transform;
            isLockedOn = true;
        }

        if (!enemiesInRange.Contains(other))
            enemiesInRange.Add(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (enemiesInRange.Contains(other))
            enemiesInRange.Remove(other);
    }

    public void Initialize(Vector2 dir)
    {
        moveDirection = dir.normalized;
    }
}
