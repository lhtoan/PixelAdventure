// using System.Collections;
// using UnityEngine;

// public class Health : MonoBehaviour
// {
//     [SerializeField] private float startingHealth = 3f;
//     public float currentHealth { get; private set; }
//     private Animator anim;
//     private bool dead;

//     [SerializeField] private float iFramesDuration = 1f;
//     [SerializeField] private int numberOfFlashes = 3;
//     private SpriteRenderer spriteRend;

//     private PlayerRespawn respawnManager;

//     private void Awake()
//     {
//         currentHealth = startingHealth;
//         spriteRend = GetComponent<SpriteRenderer>();
//         respawnManager = GetComponent<PlayerRespawn>();
//         anim = GetComponent<Animator>();
//     }

//     public void TakeDamage(float _damage)
//     {
//         if (dead) return;

//         currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

//         if (currentHealth > 0)
//         {
//             StartCoroutine(Invunerability());
//             anim.SetTrigger("hurt");
//         }
//         else
//         {
//             // Player die
//             if (!dead)
//             {
//                 dead = true;
//                 anim.SetTrigger("die");
//                 GetComponent<PlayerController>().enabled = false;


//             }


//         }
//     }

//     public void AddHealth(float _value)
//     {
//         if (dead) return;
//         currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
//     }

//     private IEnumerator Invunerability()
//     {
//         Physics2D.IgnoreLayerCollision(7, 8, true);
//         for (int i = 0; i < numberOfFlashes; i++)
//         {
//             spriteRend.color = new Color(1, 0, 0, 0.5f);
//             yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
//             spriteRend.color = Color.white;
//             yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
//         }
//         Physics2D.IgnoreLayerCollision(7, 8, false);
//     }

//     public void Respawn()
//     {
//         AddHealth(startingHealth);
//         anim.ResetTrigger("die");
//         anim.Play("Idle");
//         StartCoroutine(Invunerability());
//     }


//     public void OnPlayerDeathAnimationEnd()
//     {
//         respawnManager.PlayerDied();
//     }

// }

using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    [Header("Health")]
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
    public void TakeDamage(float _damage)
    {
        if (invulnerable) return;
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            anim.SetTrigger("hurt");
            StartCoroutine(Invunerability());
        }
        else
        {
            if (!dead)
            {
                //Deactivate all attached component classes
                foreach (Behaviour component in components)
                    component.enabled = false;

                anim.SetTrigger("die");

                dead = true;

                StartCoroutine(DieCoroutine());
            }
        }
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
}