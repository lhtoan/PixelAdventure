using UnityEngine;

public class BreakableBox : MonoBehaviour
{
    [SerializeField] private int maxHealth = 1;   // số hit để phá
    private int currentHealth;
    private bool broken;
    private Animator anim;
    private Collider2D col;

    private void Awake()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
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
            col.enabled = false; // tắt va chạm để player không kẹt
    }

    // Gọi hàm này từ animation event cuối cùng của clip "break"
    private void DestroyBox()
    {
        Destroy(gameObject);
    }
}
