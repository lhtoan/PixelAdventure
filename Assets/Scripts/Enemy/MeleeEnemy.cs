// using UnityEngine;

// public class MeleeEnemy : MonoBehaviour
// {
//     [Header("Attack Parameters")]
//     [SerializeField] private float attackCooldown;
//     [SerializeField] private float range;
//     [SerializeField] private int damage;

//     [Header("Collider Parameters")]
//     [SerializeField] private float colliderDistance;
//     [SerializeField] private BoxCollider2D boxCollider;

//     [Header("Player Layer")]
//     [SerializeField] private LayerMask targetLayer;
//     private float cooldownTimer = Mathf.Infinity;

//     //References
//     private Animator anim;
//     private Health playerHealth;
//     private EnemyPatrol enemyPatrol;

//     private void Awake()
//     {
//         anim = GetComponent<Animator>();
//         enemyPatrol = GetComponentInParent<EnemyPatrol>();
//     }

//     private void Update()
//     {
//         cooldownTimer += Time.deltaTime;

//         //Attack only when player in sight?
//         if (PlayerInSight())
//         {
//             if (cooldownTimer >= attackCooldown)
//             {
//                 cooldownTimer = 0;
//                 anim.SetTrigger("meleeAttack");
//             }
//         }

//         if (enemyPatrol != null)
//             enemyPatrol.enabled = !PlayerInSight();
//     }

//     private bool PlayerInSight()
//     {
//         RaycastHit2D hit =
//             Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
//             new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
//             0, Vector2.left, 0, targetLayer);

//         if (hit.collider != null)
//             playerHealth = hit.transform.GetComponent<Health>();


//         return hit.collider != null;
//     }
//     private void OnDrawGizmos()
//     {
//         Gizmos.color = Color.red;
//         Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
//             new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));

//     }

//     private void DamagePlayer()
//     {
//         if (PlayerInSight())
//         {
//             playerHealth.TakeDamage(damage);
//         }

//     }

//     protected virtual void OnCollisionEnter2D(Collision2D collision)
//     {
//         if (collision.collider.CompareTag("Player"))
//         {
//             collision.collider.GetComponent<Health>().TakeDamage(damage);
//         }
//     }
// }
using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float range = 0.5f;
    [SerializeField] private int damage = 10;

    [Header("Collider Parameters")]
    [SerializeField] private float colliderDistance = 0.5f;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Player Layer")]
    [SerializeField] private LayerMask targetLayer;

    private float cooldownTimer = Mathf.Infinity;

    // References
    private Animator anim;
    private Health playerHealth;
    private IEnemyMovement movement;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        movement = GetComponentInParent<IEnemyMovement>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        bool seePlayer = PlayerInSight();

        if (seePlayer && cooldownTimer >= attackCooldown)
        {
            cooldownTimer = 0;
            anim.SetTrigger("meleeAttack");

            // Stop movement during attack
            if (movement != null)
                movement.EnableMovement(false);
        }
        else
        {
            // Re-enable movement when not attacking
            if (movement != null)
                movement.EnableMovement(!seePlayer);
        }
    }

    private bool PlayerInSight()
    {
        RaycastHit2D hit =
            Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, targetLayer);

        if (hit.collider != null)
            playerHealth = hit.transform.GetComponent<Health>();

        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        if (boxCollider == null) return;

        Vector3 castDir = Vector3.right * transform.localScale.x;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            boxCollider.bounds.center + castDir * range * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range,
                        boxCollider.bounds.size.y,
                        boxCollider.bounds.size.z)
        );
    }

    // Called from Animation Event
    private void DamagePlayer()
    {
        if (PlayerInSight())
        {
            playerHealth.TakeDamage(damage);
        }
    }
}
