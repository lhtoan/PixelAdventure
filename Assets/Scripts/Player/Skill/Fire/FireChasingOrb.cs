using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
public class FireChasingOrb : MonoBehaviour
{
    [Header("Orb Movement")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float rotateSpeed = 6f;
    [SerializeField] private float moveDistance = 8f;
    [SerializeField] private bool useAutoDisable = true;
    [SerializeField] private float disableDelayAfterStop = 0.3f;

    [Header("Combat Settings")]
    [SerializeField] private float instantHitDamage = 10f;
    [SerializeField] private float damagePerSecond = 6f;

    private Transform targetEnemy = null;
    private Vector2 moveDirection = Vector2.right;

    private float traveled = 0f;
    private bool active = false;
    private bool isLockedOn = false;

    private HashSet<GameObject> burnedEnemies = new HashSet<GameObject>();
    private HashSet<GameObject> firstHitEnemies = new HashSet<GameObject>();

    private void Awake()
    {
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;
    }

    private void OnEnable()
    {
        active = true;
        traveled = 0f;

        burnedEnemies.Clear();
        firstHitEnemies.Clear();

        isLockedOn = false;

        moveDirection = Vector2.right;
        targetEnemy = FindClosestEnemy();
    }

    private void Update()
    {
        if (!active)
            return;

        // Nếu target chết hoặc disable → tìm target mới
        if (!isLockedOn) // chỉ tìm lại enemy khi không lock target
        {
            if (targetEnemy == null || !targetEnemy.gameObject.activeInHierarchy)
                targetEnemy = FindClosestEnemy();
        }

        if (targetEnemy == null)
            return;

        // Chỉ đổi hướng khi KHÔNG bị khóa target
        if (!isLockedOn)
        {
            Vector2 dir = (targetEnemy.position - transform.position).normalized;
            moveDirection = Vector2.Lerp(moveDirection, dir, rotateSpeed * Time.deltaTime).normalized;
        }

        // Di chuyển orb theo hướng đã có
        Vector3 delta = (Vector3)moveDirection * moveSpeed * Time.deltaTime;
        transform.position += delta;
        traveled += delta.magnitude;

        // Hết tầm hoạt động
        if (useAutoDisable && traveled >= moveDistance)
        {
            StartCoroutine(StopAndDisable());
            active = false;
        }
    }

    // Enemy vào vùng → KHÓA TARGET → không đổi hướng nữa
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            isLockedOn = true;
        }
    }

    // Enemy rời vùng → MỞ KHÓA TARGET → có thể chuyển hướng tiếp
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            isLockedOn = false;
        }
    }

    private Transform FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (var e in enemies)
        {
            if (!e.activeInHierarchy)
                continue;

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
        if (!active || !collision.CompareTag("Enemy"))
            return;

        var health = collision.GetComponent<Health>();

        if (health != null)
        {
            // Damage lần đầu
            if (!firstHitEnemies.Contains(collision.gameObject))
            {
                health.TakeDamage(instantHitDamage, true);
                firstHitEnemies.Add(collision.gameObject);
            }

            // Damage theo thời gian
            health.TakeDamage(damagePerSecond * Time.deltaTime, false);
        }

        // Burn effect
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

    public void Initialize(Vector2 dir) { }
}
