// using UnityEngine;

// public class BreakableBox : MonoBehaviour
// {
//     [SerializeField] private int maxHealth = 1;   // số hit để phá
//     private int currentHealth;
//     private bool broken;
//     private Animator anim;
//     private Collider2D col;

//     private void Awake()
//     {
//         currentHealth = maxHealth;
//         anim = GetComponent<Animator>();
//         col = GetComponent<Collider2D>();
//     }

//     public void TakeDamage(int damage)
//     {
//         if (broken) return;

//         currentHealth -= damage;

//         if (currentHealth <= 0)
//         {
//             Break();
//         }
//     }

//     private void Break()
//     {
//         broken = true;
//         anim.SetTrigger("break");

//         if (col != null)
//             col.enabled = false; // tắt va chạm để player không kẹt
//     }

//     // Gọi hàm này từ animation event cuối cùng của clip "break"
//     private void DestroyBox()
//     {
//         Destroy(gameObject);
//     }
// }
using UnityEngine;

public class BreakableBox : MonoBehaviour
{
    [SerializeField] private int maxHealth = 1;
    private int currentHealth;
    private bool broken;
    private Animator anim;
    private Collider2D col;
    private Rigidbody2D rb;

    private void Awake()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage)
    {
        if (broken) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Break();
        }
    }

    private void Break()
    {
        broken = true;
        anim.SetTrigger("break");

        if (col != null)
            col.enabled = false;

        // ⭐ Ngăn box rơi khi nổ
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
        }
    }

    // Gọi từ animation event cuối
    private void DestroyBox()
    {
        Destroy(gameObject);
    }
}
