using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] protected float damage;

    // Nếu collider là Trigger
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        TryDamage(collision.gameObject);
    }

    // Nếu collider KHÔNG phải Trigger
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        TryDamage(collision.collider.gameObject);
    }

    private void TryDamage(GameObject obj)
    {
        if (obj.CompareTag("Player") || obj.CompareTag("NPC"))
        {
            Health hp = obj.GetComponent<Health>();
            if (hp != null)
                hp.TakeDamage(damage);
        }
    }
}
