using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private Transform firePoint;

    [Header("Projectiles")]
    [SerializeField] private GameObject[] fireballs; // Chuột trái (3 tia)
    [SerializeField] private GameObject[] iceballs;  // Chuột phải (1 tia)

    [Header("Stamina Costs")]
    [SerializeField] private float fireStaminaCost = 1.5f;  // 🔥 Mỗi lần bắn Fire tốn 1.5
    [SerializeField] private float iceStaminaCost = 3f;     // ❄️ Mỗi lần bắn Ice tốn 3

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

        // 🔥 Chuột trái → Bắn 3 tia Fireball
        if (Input.GetMouseButton(0) && cooldownTimer > attackCooldown)
        {
            TryFireAttack();
        }

        // ❄️ Chuột phải → Bắn Iceball
        if (Input.GetMouseButton(1) && cooldownTimer > attackCooldown)
        {
            TryIceAttack();
        }
    }

    // -------------------------------------------------------------
    // 🔥 Fire Attack
    // -------------------------------------------------------------
    private void TryFireAttack()
    {
        // Kiểm tra stamina
        if (playerController.CanUseStamina(fireStaminaCost))
        {
            playerController.UseStamina(fireStaminaCost);
            FireAttack();
        }
        else
        {
            Debug.Log("❌ Không đủ stamina để bắn Fire!");
        }
    }

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

    // -------------------------------------------------------------
    // ❄️ Ice Attack
    // -------------------------------------------------------------
    private void TryIceAttack()
    {
        // Kiểm tra stamina
        if (playerController.CanUseStamina(iceStaminaCost))
        {
            playerController.UseStamina(iceStaminaCost);
            IceAttack();
        }
        else
        {
            Debug.Log("❌ Không đủ stamina để bắn Ice!");
        }
    }

    private void IceAttack()
    {
        anim.SetTrigger("attack");
        cooldownTimer = 0;

        float dir = Mathf.Sign(transform.localScale.x);
        int index = FindInactive(iceballs);

        GameObject iceball = iceballs[index];
        iceball.transform.position = firePoint.position;
        iceball.GetComponent<Projecttile>().SetDirection(dir);

        // Gắn tag “Ice” để Projecttile biết đây là đạn băng
        iceball.tag = "Ice";
    }

    // -------------------------------------------------------------
    // 🔍 Tìm viên đạn trống trong pool
    // -------------------------------------------------------------
    private int FindInactive(GameObject[] pool)
    {
        for (int i = 0; i < pool.Length; i++)
        {
            if (!pool[i].activeInHierarchy)
                return i;
        }
        return 0;
    }
}
