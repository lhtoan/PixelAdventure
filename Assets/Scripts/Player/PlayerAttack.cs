// using UnityEngine;

// public class PlayerAttack : MonoBehaviour
// {
//     [SerializeField] private float attackCooldown;
//     [SerializeField] private Transform firePoint;
//     [SerializeField] private GameObject[] fireballs;
//     private Animator anim;
//     private PlayerController playerController;
//     private float cooldownTimer = Mathf.Infinity;

//     private void Awake()
//     {
//         anim = GetComponent<Animator>();
//         playerController = GetComponent<PlayerController>();
//     }

//     private void Update()
//     {
//         if (Input.GetMouseButton(0) && cooldownTimer > attackCooldown && playerController.canAttack())
//         {
//             playerController.UseAttackStamina();
//             Attack();
//         }
//         cooldownTimer += Time.deltaTime;
//     }

//     private void Attack()
//     {
//         anim.SetTrigger("attack");
//         cooldownTimer = 0;

//         fireballs[FindFireball()].transform.position = firePoint.position;
//         fireballs[FindFireball()].GetComponent<Projecttile>().SetDirection(Mathf.Sign(transform.localScale.x));
//     }

//     // private void Attack()
//     // {
//     //     anim.SetTrigger("attack");
//     //     cooldownTimer = 0;

//     //     float dir = Mathf.Sign(transform.localScale.x);

//     //     // Góc bắn lệch (có thể chỉnh tuỳ bạn)
//     //     float[] angles = { 0f, 15f, -15f };

//     //     for (int i = 0; i < angles.Length; i++)
//     //     {
//     //         int fireballIndex = FindFireball();
//     //         GameObject fireball = fireballs[fireballIndex];
//     //         fireball.transform.position = firePoint.position;
//     //         fireball.GetComponent<Projecttile>().SetDirection(dir, angles[i]);
//     //     }
//     // }


//     private int FindFireball()
//     {
//         for (int i = 0; i < fireballs.Length; i++)
//         {
//             if (!fireballs[i].activeInHierarchy)
//                 return i;
//         }
//         return 0;
//     }
// }

using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private Transform firePoint;

    [Header("Projectiles")]
    [SerializeField] private GameObject[] fireballs; // Dùng cho Left Click (3 tia)
    [SerializeField] private GameObject[] iceballs;  // Dùng cho Right Click (1 đòn)

    private Animator anim;
    private PlayerController playerController;
    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        // Chuột trái → bắn 3 tia Fireball
        if (Input.GetMouseButton(0) && cooldownTimer > attackCooldown && playerController.canAttack())
        {
            playerController.UseAttackStamina();
            FireAttack();
        }

        // Chuột phải → bắn 1 đòn Iceball
        if (Input.GetMouseButton(1) && cooldownTimer > attackCooldown && playerController.canAttack())
        {
            playerController.UseAttackStamina();
            IceAttack();
        }
    }

    // 🔥 Fire Attack (3 tia)
    private void FireAttack()
    {
        anim.SetTrigger("attack");
        cooldownTimer = 0;

        float dir = Mathf.Sign(transform.localScale.x);
        float[] angles = { 0f, 15f, -15f }; // tỏa 3 hướng

        for (int i = 0; i < angles.Length; i++)
        {
            int index = FindInactive(fireballs);
            GameObject fireball = fireballs[index];
            fireball.transform.position = firePoint.position;
            fireball.GetComponent<Projecttile>().SetDirection(dir, angles[i]);
        }
    }

    // ❄️ Ice Attack (1 đòn tập trung)
    private void IceAttack()
    {
        anim.SetTrigger("attack");
        cooldownTimer = 0;

        float dir = Mathf.Sign(transform.localScale.x);
        int index = FindInactive(iceballs);

        GameObject iceball = iceballs[index];
        iceball.transform.position = firePoint.position;
        iceball.GetComponent<Projecttile>().SetDirection(dir);
    }

    // 🔍 Tìm viên đạn trống
    private int FindInactive(GameObject[] pool)
    {
        for (int i = 0; i < pool.Length; i++)
        {
            if (!pool[i].activeInHierarchy)
                return i;
        }
        return 0; // nếu tất cả đều đang dùng
    }
}
