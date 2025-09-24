// using System.Collections;
// using UnityEngine;

// public class Health : MonoBehaviour
// {
//     [SerializeField] private float startingHealth;
//     public float currentHealth
//     {
//         get;
//         private set;
//     }
//     private bool dead;
//     [SerializeField] private float iFramesDuration;
//     [SerializeField] private int numberOfFlashes;
//     private SpriteRenderer spriteRend;

//     private void Awake()
//     {
//         currentHealth = startingHealth;
//         spriteRend = GetComponent<SpriteRenderer>();
//     }

//     public void TakeDamage(float _damage)
//     {
//         currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

//         if (currentHealth > 0)
//         {
//             //player hurt
//             StartCoroutine(Invunerability());
//         }
//         else
//         {
//             //player die
//         }
//     }

//     // private void Update()
//     // {
//     //     if (Input.GetKeyDown(KeyCode.E))
//     //     {
//     //         TakeDamage(1);
//     //     }
//     // }

//     public void AddHealth(float _value)
//     {
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
//     }
// }

using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth = 3f;
    public float currentHealth { get; private set; }
    private bool dead;

    [SerializeField] private float iFramesDuration = 1f;
    [SerializeField] private int numberOfFlashes = 3;
    private SpriteRenderer spriteRend;

    private PlayerRespawn respawnManager;

    private void Awake()
    {
        currentHealth = startingHealth;
        spriteRend = GetComponent<SpriteRenderer>();
        respawnManager = GetComponent<PlayerRespawn>();
    }

    public void TakeDamage(float _damage)
    {
        if (dead) return;

        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            // Player bị thương
            StartCoroutine(Invunerability());
        }
        else
        {
            // Player chết
            dead = true;
            respawnManager.PlayerDied(); // báo cho Respawn xử lý mạng và checkpoint
        }
    }

    public void AddHealth(float _value)
    {
        if (dead) return;
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }

    private IEnumerator Invunerability()
    {
        Physics2D.IgnoreLayerCollision(7, 8, true);
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(7, 8, false);
    }

    public void Respawn()
    {
        dead = false;
        currentHealth = startingHealth;
        gameObject.SetActive(true);
    }
}
