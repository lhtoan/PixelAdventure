using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private Transform firePoint;

    [Header("Projectiles")]
    [SerializeField] private GameObject[] fireballs; // Fire mode
    [SerializeField] private GameObject[] iceballs;  // Ice mode

    [Header("Stamina Costs")]
    [SerializeField] private float fireCost = 1.5f;
    [SerializeField] private float iceCost = 3f;

    private Animator anim;
    private PlayerStamina stamina;
    private float cooldownTimer = Mathf.Infinity;

    public enum Element { Fire, Ice }
    [SerializeField] private Element currentElement = Element.Fire; //mặc định Fire
    public Element CurrentElement => currentElement;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        stamina = GetComponent<PlayerStamina>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        // Đổi hệ bằng chuột phải
        if (Input.GetMouseButtonDown(1))
        {
            currentElement = (currentElement == Element.Fire) ? Element.Ice : Element.Fire;
            Debug.Log("Chuyển sang hệ: " + currentElement);
        }

        // 🔫 Chuột trái = tấn công chính
        if (Input.GetMouseButton(0) && cooldownTimer > attackCooldown)
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        switch (currentElement)
        {
            case Element.Fire:
                if (!stamina.CanUse(fireCost))
                {
                    // Debug.Log("❌ Không đủ stamina để bắn Fire!");
                    return;
                }
                stamina.Use(fireCost);
                FireAttack();
                break;

            case Element.Ice:
                if (!stamina.CanUse(iceCost))
                {
                    // Debug.Log("❌ Không đủ stamina để bắn Ice!");
                    return;
                }
                stamina.Use(iceCost);
                IceAttack();
                break;
        }
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
