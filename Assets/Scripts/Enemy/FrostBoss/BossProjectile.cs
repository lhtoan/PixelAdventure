// using UnityEngine;

// public class BossProjectile : MonoBehaviour
// {
//     [Header("Projectile Settings")]
//     [SerializeField] private float speed = 6f;
//     [SerializeField] private float maxDistance = 5f;
//     [SerializeField] private float damage = 5f;
//     [SerializeField] private float resetTime = 3f;

//     private Vector2 moveDir;
//     private Vector3 startPos;
//     private float lifetime;

//     private BoxCollider2D box;
//     private bool hasHit = false;

//     private void Awake()
//     {
//         box = GetComponent<BoxCollider2D>();
//     }

//     private void OnEnable()
//     {
//         startPos = transform.position;
//         lifetime = 0f;
//         hasHit = false;

//         if (box != null)
//             box.enabled = true;
//     }

//     private void Update()
//     {
//         if (hasHit) return;

//         // ⭐ Move theo hướng
//         transform.Translate(moveDir * speed * Time.deltaTime, Space.World);

//         lifetime += Time.deltaTime;

//         // ⭐ Auto off
//         if (lifetime >= resetTime || Vector3.Distance(startPos, transform.position) >= maxDistance)
//         {
//             gameObject.SetActive(false);
//         }
//     }

//     // ==========================================================
//     //      NHẬN HƯỚNG BẮN
//     // ==========================================================
//     public void SetDirection(Vector2 dir)
//     {
//         moveDir = dir.normalized;

//         // Rotate sprite theo hướng
//         float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
//         transform.rotation = Quaternion.Euler(0, 0, angle);
//     }

//     // ==========================================================
//     //      VA CHẠM
//     // ==========================================================
//     private void OnTriggerEnter2D(Collider2D col)
//     {
//         if (hasHit) return;

//         if (col.CompareTag("Player"))
//         {
//             hasHit = true;

//             col.GetComponent<Health>()?.TakeDamage(damage);

//             if (box != null)
//                 box.enabled = false;

//             gameObject.SetActive(false);
//         }
//     }

//     public void SetSpeed(float s)
//     {
//         speed = s;
//     }

// }
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 6f;
    [SerializeField] private float damage = 5f;

    private Vector2 moveDir;

    private BoxCollider2D box;
    private Animator anim;

    private bool hasHit = false;

    private void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        hasHit = false;
        if (box != null) box.enabled = true;
    }

    private void Update()
    {
        if (hasHit) return;

        // ⭐ Projectile di chuyển bình thường
        transform.Translate(moveDir * speed * Time.deltaTime, Space.World);
    }

    // ==========================================================
    //      NHẬN HƯỚNG BẮN
    // ==========================================================
    public void SetDirection(Vector2 dir)
    {
        moveDir = dir.normalized;

        // Rotate sprite theo hướng
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // ==========================================================
    //      VA CHẠM
    // ==========================================================
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (hasHit) return;

        if (col.CompareTag("Player"))
        {
            col.GetComponent<Health>()?.TakeDamage(damage);
            Hit();
        }
        else if (col.CompareTag("Ground"))
        {
            Hit();
        }
    }

    // ⭐ Xử lý hit chung
    private void Hit()
    {
        hasHit = true;

        if (box != null)
            box.enabled = false;

        // ⭐ Chạy animation explode
        if (anim != null)
            anim.SetTrigger("explode");
        else
            gameObject.SetActive(false);
    }

    // ⭐ Animation Event gọi lúc animation explode kết thúc
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void SetSpeed(float s)
    {
        speed = s;
    }
}
