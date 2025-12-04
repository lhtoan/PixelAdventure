// using UnityEngine;

// public class BossController : MonoBehaviour
// {
//     public Transform target;
//     public float attackDistance = 2f;
//     public float attackCooldown = 1.5f;

//     private Animator anim;
//     private float cooldownTimer = 0f;

//     private void Awake()
//     {
//         anim = GetComponent<Animator>();
//     }

//     private void Update()
//     {
//         cooldownTimer -= Time.deltaTime;

//         if (target == null) return;

//         float dist = Vector2.Distance(transform.position, target.position);

//         // Nếu ở gần player
//         if (dist <= attackDistance)
//         {
//             TryAttack();
//         }
//     }

//     private void TryAttack()
//     {
//         if (cooldownTimer > 0) return;

//         cooldownTimer = attackCooldown;

//         anim.SetTrigger("attack");   // chạy animation attack
//     }
// }
