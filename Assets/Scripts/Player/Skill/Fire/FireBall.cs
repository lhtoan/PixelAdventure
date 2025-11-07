using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Fireball : MonoBehaviour
{
    [Header("Fireball Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifeTime = 3f;

    private Vector2 moveDir;
    private bool active = false;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        var col = GetComponent<Collider2D>();
        col.isTrigger = true; // 🔹 Để xuyên qua enemy

        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    public void Launch(Vector2 direction, float? overrideSpeed = null, float? overrideDamage = null)
    {
        moveDir = direction.normalized;
        if (overrideSpeed.HasValue) speed = overrideSpeed.Value;
        if (overrideDamage.HasValue) damage = overrideDamage.Value;

        active = true;
        gameObject.SetActive(true);

        rb.linearVelocity = moveDir * speed;

        StopAllCoroutines();
        StartCoroutine(AutoDisable());
    }

    private IEnumerator AutoDisable()
    {
        yield return new WaitForSeconds(lifeTime);
        Deactivate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!active) return;

        // 👹 Nếu trúng enemy → gây damage & burn, nhưng KHÔNG biến mất
        if (collision.CompareTag("Enemy"))
        {
            var health = collision.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            var burn = collision.GetComponent<BurnEnemy>();
            if (burn != null)
            {
                burn.TriggerBurn();
            }

            // ❗ KHÔNG gọi Deactivate() ở đây → xuyên qua được nhiều enemy
        }

        // 🧱 Nếu trúng tường, vật thể cứng → tắt đạn
        // else if (collision.CompareTag("Ground"))
        // {
        //     Deactivate();
        // }
    }

    private void Deactivate()
    {
        active = false;
        rb.linearVelocity = Vector2.zero;
        gameObject.SetActive(false);
    }
}
