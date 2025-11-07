using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class HellfireTornado : MonoBehaviour
{
    [Header("Tornado Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float moveDistance = 6f;
    [SerializeField] private bool useAutoDisable = true;
    [SerializeField] private float disableDelayAfterStop = 0.3f;

    [Header("Combat Settings")]
    [SerializeField] private float instantHitDamage = 10f;  // 💥 Damage ngay lần đầu chạm
    [SerializeField] private float damagePerSecond = 6f;    // 🔥 Damage liên tục (DOT)
    [SerializeField] private float burnDuration = 3f;

    private Vector2 moveDir = Vector2.right;
    private bool active = false;
    private Vector3 startPosition;
    private float traveled = 0f;
    private BoxCollider2D boxCollider;

    private HashSet<GameObject> burnedEnemies = new HashSet<GameObject>();
    private HashSet<GameObject> firstHitEnemies = new HashSet<GameObject>(); // 💥 Lưu enemy đã dính hit đầu tiên

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;
    }

    private void OnEnable()
    {
        active = true;
        traveled = 0f;
        startPosition = transform.position;
        burnedEnemies.Clear();
        firstHitEnemies.Clear();
    }

    public void Initialize(Vector2 direction)
    {
        moveDir = direction.sqrMagnitude > 0.001f ? direction.normalized : Vector2.right;
    }

    private void Update()
    {
        if (!active) return;

        Vector3 delta = (Vector3)moveDir * moveSpeed * Time.deltaTime;
        transform.Translate(delta, Space.World);
        traveled += delta.magnitude;

        if (useAutoDisable && traveled >= moveDistance)
        {
            StartCoroutine(StopAndDisable());
            active = false;
        }
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
            // 💥 Hit đầu tiên (một lần duy nhất)
            if (!firstHitEnemies.Contains(collision.gameObject))
            {
                health.TakeDamage(instantHitDamage, true); // bật anim hurt
                firstHitEnemies.Add(collision.gameObject);
                Debug.Log($"🔥 Tornado first hit {collision.name} for {instantHitDamage} damage!");
            }

            // 🔥 Damage liên tục theo thời gian (DOT)
            health.TakeDamage(damagePerSecond * Time.deltaTime, false);
        }

        // 🔥 Gây Burn (1 lần duy nhất)
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
}
