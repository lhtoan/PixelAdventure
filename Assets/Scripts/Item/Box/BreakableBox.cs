// using UnityEngine;

// public class BreakableBox : MonoBehaviour
// {
//     [SerializeField] private int maxHealth = 1;
//     private int currentHealth;
//     private bool broken;
//     private Animator anim;
//     private Collider2D col;
//     private Rigidbody2D rb;

//     private void Awake()
//     {
//         currentHealth = maxHealth;
//         anim = GetComponent<Animator>();
//         col = GetComponent<Collider2D>();
//         rb = GetComponent<Rigidbody2D>();
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
//             col.enabled = false;

//         // ⭐ Ngăn box rơi khi nổ
//         if (rb != null)
//         {
//             rb.bodyType = RigidbodyType2D.Kinematic;
//             rb.linearVelocity = Vector2.zero;
//         }
//     }

//     // Gọi từ animation event cuối
//     private void DestroyBox()
//     {
//         Destroy(gameObject);
//     }
// }
using UnityEngine;

public class BreakableBox : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 1;
    private int currentHealth;
    private bool broken;

    [Header("Components")]
    private Animator anim;
    private Collider2D col;
    private Rigidbody2D rb;

    [Header("Objects to Toggle")]
    public GameObject deleteItem;   // ⭐ Object sẽ ẩn
    public GameObject item;         // ⭐ Object sẽ hiện

    [Header("Toggle Settings")]
    public bool hideDeleteItem = true;   // box vỡ thì ẩn deleteItem?
    public bool showItem = false;        // box vỡ thì hiện item?

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

        if (anim != null)
            anim.SetTrigger("break");

        if (col != null)
            col.enabled = false;

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
        }

        // ⭐ ẨN deleteItem nếu được bật
        if (hideDeleteItem && deleteItem != null)
            deleteItem.SetActive(false);

        // ⭐ HIỆN item nếu được bật
        if (showItem && item != null)
            item.SetActive(true);
    }

    // Gọi cuối animation
    private void DestroyBox()
    {
        Destroy(gameObject);
    }
}
