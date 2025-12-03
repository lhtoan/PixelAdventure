using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private bool useInvulnerability = true;
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }
    private Animator anim;
    private bool dead;

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRend;

    [Header("Components")]
    [SerializeField] private Behaviour[] components;
    private bool invulnerable;
    private bool shieldActive;

    private PlayerRespawn respawnManager;

    // [Header("Death Sound")]
    // [SerializeField] private AudioClip deathSound;
    // [SerializeField] private AudioClip hurtSound;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
        respawnManager = GetComponent<PlayerRespawn>();
    }
    // public void TakeDamage(float _damage)
    // {

    //     if (shieldActive) return;
    //     if (invulnerable) return;
    //     currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

    //     if (currentHealth > 0)
    //     {
    //         if (HasParameter(anim, "hurt"))
    //             anim.SetTrigger("hurt");

    //         Debug.Log(currentHealth);

    //         StartCoroutine(Invunerability());
    //     }
    //     else
    //     {
    //         if (!dead)
    //         {
    //             // Deactivate all attached component classes
    //             foreach (Behaviour component in components)
    //                 component.enabled = false;

    //             if (HasParameter(anim, "die"))
    //                 anim.SetTrigger("die");


    //             dead = true;

    //             StartCoroutine(DieCoroutine());
    //         }
    //     }
    // }

    public void TakeDamage(float _damage)
    {
        if (shieldActive) return;

        // chỉ player mới dùng invulnerability
        if (useInvulnerability && invulnerable) return;

        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            if (HasParameter(anim, "hurt"))
                anim.SetTrigger("hurt");

            // Debug.Log(currentHealth);

            if (useInvulnerability)
                StartCoroutine(Invunerability());
        }
        else
        {
            if (!dead)
            {
                foreach (Behaviour component in components)
                    component.enabled = false;

                if (HasParameter(anim, "die"))
                    anim.SetTrigger("die");

                dead = true;

                StartCoroutine(DieCoroutine());
            }
        }
    }

    // public void TakeDamage(float _damage, bool triggerAnimation = true)
    // {

    //     if (shieldActive) return;
    //     if (invulnerable && triggerAnimation) return;

    //     currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

    //     if (currentHealth > 0)
    //     {
    //         if (triggerAnimation && HasParameter(anim, "hurt"))
    //             anim.SetTrigger("hurt");

    //         if (triggerAnimation)
    //             StartCoroutine(Invunerability());

    //         Debug.Log(currentHealth);
    //     }
    //     else if (!dead)
    //     {
    //         foreach (Behaviour component in components)
    //             component.enabled = false;

    //         if (HasParameter(anim, "die"))
    //             anim.SetTrigger("die");

    //         dead = true;
    //         StartCoroutine(DieCoroutine());
    //     }
    // }
    public void TakeDamage(float _damage, bool triggerAnimation = true)
    {
        if (shieldActive) return;

        // chỉ player mới dùng invulnerability
        if (useInvulnerability && invulnerable && triggerAnimation)
            return;

        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            if (triggerAnimation && HasParameter(anim, "hurt"))
                anim.SetTrigger("hurt");

            if (triggerAnimation && useInvulnerability)
                StartCoroutine(Invunerability());

            // Debug.Log(currentHealth);
        }
        else if (!dead)
        {
            foreach (Behaviour component in components)
                component.enabled = false;

            if (HasParameter(anim, "die"))
                anim.SetTrigger("die");

            dead = true;
            StartCoroutine(DieCoroutine());
        }
    }


    // Kiểm tra player có đang bật khiên hay không nếu có không trừ máu
    public void SetShieldProtection(bool state)
    {
        shieldActive = state;
        // if (state)
        //     Debug.Log("🧊 Player đang được Ice Shield bảo vệ!");
        // else
        //     Debug.Log("🧊 Ice Shield tắt bảo vệ!");
    }


    private IEnumerator DieCoroutine()
    {
        // Đợi thời gian animation die phát xong (chỉnh đúng thời lượng clip của bạn)
        yield return new WaitForSeconds(1.2f);

        // Gọi hệ thống respawn
        if (respawnManager != null)
            respawnManager.RespawnCheck();
    }

    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }

    // public void IncreaseMaxHealth(int amount)
    // {
    //     startingHealth += amount;
    //     currentHealth += amount;

    //     // Không cho máu vượt max mới
    //     currentHealth = Mathf.Clamp(currentHealth, 0, startingHealth);
    // }
    public void IncreaseMaxHealth(int amount)
    {
        startingHealth += amount;
        currentHealth += amount;

        currentHealth = Mathf.Clamp(currentHealth, 0, startingHealth);

        var hb = FindFirstObjectByType<HealthBar>();
        if (hb != null)
            hb.UpdateTotalBar();
    }



    private IEnumerator Invunerability()
    {
        invulnerable = true;
        Physics2D.IgnoreLayerCollision(10, 11, true);
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(10, 11, false);
        invulnerable = false;
    }
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    //Respawn
    public void Respawn()
    {
        dead = false;
        currentHealth = startingHealth;

        // Reset animation về trạng thái ban đầu
        anim.ResetTrigger("die");
        anim.ResetTrigger("hurt");
        anim.Play("Idle");

        spriteRend.color = Color.white;

        // Bật lại các thành phần
        foreach (Behaviour component in components)
            component.enabled = true;

        StartCoroutine(Invunerability());
    }

    private bool HasParameter(Animator animator, string paramName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }

    public float GetStartingHealth() => startingHealth;


}