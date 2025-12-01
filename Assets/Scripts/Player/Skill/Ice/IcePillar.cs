// using UnityEngine;

// public class IceSpike : MonoBehaviour
// {
//     [SerializeField] private int damage = 10; // sát thương mỗi cột băng
//     [SerializeField] private float hitForce = 2f; // lực đẩy nhẹ (tùy chọn)

//     private bool hasHit = false; // tránh gây damage nhiều lần

//     private void OnCollisionEnter2D(Collision2D collision)
//     {
//         if (hasHit) return;

//         if (collision.collider.CompareTag("Enemy"))
//         {
//             hasHit = true;

//             // ✅ Gây sát thương
//             Health enemyHealth = collision.collider.GetComponent<Health>();
//             if (enemyHealth != null)
//             {
//                 enemyHealth.TakeDamage(damage);
//             }

//             // ✅ Kích hoạt đóng băng
//             FreezeEnemy freeze = collision.collider.GetComponent<FreezeEnemy>();
//             if (freeze != null)
//             {
//                 // Đặt stack lên 3 ngay lập tức
//                 for (int i = 0; i < 3; i++)
//                     freeze.TriggerIceHit();
//             }

//             // ✅ Thêm hiệu ứng va chạm (nếu muốn)
//             Rigidbody2D rb = collision.collider.attachedRigidbody;
//             if (rb != null)
//                 rb.AddForce(Vector2.up * hitForce, ForceMode2D.Impulse);
//         }
//     }

//     private void OnEnable()
//     {
//         hasHit = false; // reset mỗi lần spike được bật lại
//     }
// }
using UnityEngine;

public class IceSpike : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float hitForce = 2f;

    private bool hasHit = false;
    private PlayerAttack attacker;   // ⭐ được set khi spawn

    public void SetAttacker(PlayerAttack pa)
    {
        attacker = pa;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;
        if (!collision.CompareTag("Enemy")) return;
        if (attacker == null) return;   // tránh null

        hasHit = true;

        FreezeEnemy freeze = collision.GetComponent<FreezeEnemy>();

        if (freeze != null)
        {
            // 3 hit apply ice damage
            for (int i = 0; i < 3; i++)
                freeze.ApplyIceDamage(damage, attacker);
        }

        Rigidbody2D rb = collision.attachedRigidbody;
        if (rb != null)
            rb.AddForce(Vector2.up * hitForce, ForceMode2D.Impulse);
    }

    private void OnEnable()
    {
        hasHit = false;
    }
}
