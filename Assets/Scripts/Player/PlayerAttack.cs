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
    [SerializeField] private float fireCost = 1.5f;
    [SerializeField] private float iceCost = 3f;

    private Animator anim;
    private PlayerStamina stamina;
    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        stamina = GetComponent<PlayerStamina>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (Input.GetMouseButton(0) && cooldownTimer > attackCooldown)
            TryFireAttack();

        if (Input.GetMouseButton(1) && cooldownTimer > attackCooldown)
            TryIceAttack();
    }

    private void TryFireAttack()
    {
        if (!stamina.CanUse(fireCost))
        {
            Debug.Log("❌ Không đủ stamina để bắn Fire!");
            return;
        }

        stamina.Use(fireCost);
        FireAttack();
    }

    private void FireAttack()
    {
        anim.SetTrigger("attack");
        cooldownTimer = 0;

        float dir = Mathf.Sign(transform.localScale.x);
        float[] angles = { 0f, 20f, -20f };

        foreach (float angle in angles)
        {
            int index = FindInactive(fireballs);
            GameObject fireball = fireballs[index];
            fireball.transform.position = firePoint.position;
            fireball.GetComponent<Projecttile>().SetDirection(dir, angle);
            fireball.tag = "Fire";
        }
    }

    private void TryIceAttack()
    {
        if (!stamina.CanUse(iceCost))
        {
            Debug.Log("❌ Không đủ stamina để bắn Ice!");
            return;
        }

        stamina.Use(iceCost);
        IceAttack();
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
        iceball.tag = "Ice";
    }

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
