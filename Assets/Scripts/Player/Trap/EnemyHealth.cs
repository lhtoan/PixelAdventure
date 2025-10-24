using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 2f;
    private float currentHealth;
    private bool dead;
    private Animator anim;

    private void Awake()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        if (dead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        dead = true;

        anim.SetTrigger("die");

        
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        Destroy(gameObject, 0.6f);
    }
}
