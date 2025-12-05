using UnityEngine;

public class BossAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float damage = 0.5f;
    public float attackCooldown = 1f;

    [Header("Hitbox Parameters")]
    public float range = 1f;
    public float colliderDistance = 0.5f;
    public BoxCollider2D boxCollider;
    public LayerMask targetLayer;

    private bool hasAttackedInThisEntry = false;

    private float cooldownTimer = Mathf.Infinity;
    private Animator anim;
    private Health playerHealth;
    private SpriteRenderer sr;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        // Nếu player KHÔNG ở trong vùng → reset
        if (!PlayerInRange())
            hasAttackedInThisEntry = false;
    }

    public bool ShouldAttackOnce()
    {
        return PlayerInRange() && !hasAttackedInThisEntry && cooldownTimer >= attackCooldown;
    }

    public void MarkAttackDone()
    {
        hasAttackedInThisEntry = true;
        cooldownTimer = 0f;
    }

    // Animation Event
    public void DoDamage()
    {
        if (PlayerInRange())
        {
            playerHealth.TakeDamage(damage);
        }
    }

    // Animation Event cuối animation
    public void EndAttack()
    {
        anim.SetBool("attack", false);
    }

    private bool PlayerInRange()
    {
        float dir = sr.flipX ? 1f : -1f;

        Vector3 boxCenter =
            boxCollider.bounds.center +
            Vector3.right * dir * range * colliderDistance;

        Vector3 boxSize = new Vector3(
            boxCollider.bounds.size.x * range,
            boxCollider.bounds.size.y,
            boxCollider.bounds.size.z
        );

        Collider2D hit = Physics2D.OverlapBox(boxCenter, boxSize, 0, targetLayer);

        if (hit)
        {
            playerHealth = hit.GetComponent<Health>();
            return true;
        }
        return false;
    }

    public bool IsPlayerInAttackRange()
    {
        return PlayerInRange();
    }

    private void OnDrawGizmos()
    {
        if (boxCollider == null) return;
        if (sr == null) sr = GetComponent<SpriteRenderer>();

        float dir = sr.flipX ? 1f : -1f;

        Vector3 center =
            boxCollider.bounds.center +
            Vector3.right * dir * range * colliderDistance;

        Vector3 size = new Vector3(
            boxCollider.bounds.size.x * range,
            boxCollider.bounds.size.y,
            boxCollider.bounds.size.z
        );

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, size);
    }

    public bool TryPerformAttack()
    {

        // nếu không trong vùng attack → không đánh
        if (!PlayerInRange())
        {
            return false;
        }

        // nếu đã đánh trong lần vào vùng này → không đánh nữa
        if (hasAttackedInThisEntry)
        {
            return false;
        }

        // kiểm cooldown
        if (cooldownTimer < attackCooldown)
        {
            return false;
        }

        // OK → đánh

        hasAttackedInThisEntry = true;
        cooldownTimer = 0f;

        anim.SetBool("attack", true);
        return true;
    }
    public bool IsAttacking => anim.GetBool("attack");


}
