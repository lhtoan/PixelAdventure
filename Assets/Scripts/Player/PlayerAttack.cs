using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private Transform firePoint;

    [Header("Projectiles")]
    [SerializeField] private GameObject[] fireballs;
    [SerializeField] private GameObject[] iceballs;

    [Header("Stamina Costs")]
    [SerializeField] private float fireCost = 1.5f;
    [SerializeField] private float iceCost = 3f;

    [Header("Burn Bonus")]
    public float burnDamageBonus = 0f;
    public float burnDurationBonus = 0f;

    private Animator anim;
    private PlayerStamina stamina;
    public bool inputLocked = false;

    private float cooldownTimer = Mathf.Infinity;

    public enum Element { Fire, Ice }
    [SerializeField] private Element currentElement = Element.Fire;
    public Element CurrentElement => currentElement;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        stamina = GetComponent<PlayerStamina>();

        if (audioManager == null)
        {
            GameObject gm = GameObject.FindGameObjectWithTag("GameManager");
            if (gm != null) audioManager = gm.GetComponent<AudioManager>();
        }
    }

    private void Update()
    {
        if (inputLocked) return;

        cooldownTimer += Time.deltaTime;

        if (Input.GetMouseButtonDown(1))
        {
            Element oldElement = currentElement;
            currentElement = (currentElement == Element.Fire) ? Element.Ice : Element.Fire;

            if (currentElement != oldElement)
            {
                FindFirstObjectByType<UI_SkillBar>()?.UpdateElementUI(currentElement);
                FindFirstObjectByType<UI_ElementPanel>()?.UpdateElement(currentElement);
            }
        }

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
                if (!stamina.CanUse(fireCost)) return;
                stamina.Use(fireCost);
                FireAttack();
                break;

            case Element.Ice:
                if (!stamina.CanUse(iceCost)) return;
                stamina.Use(iceCost);
                IceAttack();
                break;
        }
    }

    // FIRE — bắn theo chuột
    private void FireAttack()
    {
        anim.SetTrigger("attack");
        cooldownTimer = 0;

        if (audioManager != null && audioManager.fireAttackClip != null)
            audioManager.PlaySFX(audioManager.fireAttackClip, 0.5f);

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        Vector2 shootDir = (mouseWorldPos - firePoint.position).normalized;

        int index = FindInactive(fireballs);
        GameObject fireball = fireballs[index];

        fireball.transform.position = firePoint.position;

        Projecttile proj = fireball.GetComponent<Projecttile>();
        proj.SetAttacker(this);
        proj.SetDirection(shootDir);

        fireball.tag = "Fire";
    }


    private void IceAttack()
    {
        anim.SetTrigger("attack");
        cooldownTimer = 0;

        if (audioManager != null && audioManager.iceAttackClip != null)
            audioManager.PlaySFX(audioManager.iceAttackClip, 0.5f);

        // 1. Lấy vị trí chuột thế giới
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        // 2. Hướng từ firePoint → chuột
        Vector2 shootDir = (mouseWorldPos - firePoint.position).normalized;

        // 3. Lấy projectile từ pool
        int index = FindInactive(iceballs);
        GameObject iceball = iceballs[index];

        iceball.transform.position = firePoint.position;

        // 4. Gán attacker + đặt hướng
        Projecttile proj = iceball.GetComponent<Projecttile>();
        proj.SetAttacker(this);
        proj.SetDirection(shootDir);

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
