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
