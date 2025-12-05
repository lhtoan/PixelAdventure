// using UnityEngine;
// using System.Collections;

// public class BurnEnemy : MonoBehaviour
// {
//     [Header("Fire Settings")]
//     [SerializeField] private float burnDuration = 3f;    // thời gian cháy
//     [SerializeField] private float burnDamagePerSecond = 5f; // sát thương mỗi giây

//     private bool isBurning = false;
//     private SpriteRenderer sr;
//     private Health health;

//     private void Awake()
//     {
//         sr = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
//         health = GetComponent<Health>();
//     }

//     // 🔥 Gọi khi trúng Fireball
//     public void TriggerBurn()
//     {
//         if (health != null && health.currentHealth <= 0) return;
//         if (!isBurning)
//             StartCoroutine(Burn());
//     }

//     private IEnumerator Burn()
//     {
//         isBurning = true;
//         float timer = 0f;

//         if (sr != null)
//             sr.color = new Color(1f, 0.5f, 0.2f);

//         while (timer < burnDuration)
//         {
//             if (health == null || health.currentHealth <= 0)
//                 break;

//             // Gây sát thương mà KHÔNG bật animation “hurt”
//             health.TakeDamage(burnDamagePerSecond * Time.deltaTime, false);

//             timer += Time.deltaTime;
//             yield return null;
//         }

//         if (sr != null)
//             sr.color = Color.white;

//         isBurning = false;
//     }

// }
// using UnityEngine;
// using System.Collections;

// public class BurnEnemy : MonoBehaviour
// {
//     [Header("Fire Settings")]
//     [SerializeField] private float burnDuration = 3f;
//     [SerializeField] private float burnDamagePerSecond = 5f;

//     private bool isBurning = false;
//     private SpriteRenderer sr;
//     private Health health;

//     private void Awake()
//     {
//         sr = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
//         health = GetComponent<Health>();
//     }

//     public void TriggerBurn(PlayerAttack attacker)
//     {
//         if (health != null && health.currentHealth <= 0) return;
//         if (!isBurning)
//             StartCoroutine(Burn(attacker));
//     }

//     private IEnumerator Burn(PlayerAttack attacker)
//     {
//         isBurning = true;
//         float timer = 0f;

//         float realDuration = burnDuration;
//         float realDamage = burnDamagePerSecond;

//         // ⭐ Chỉ áp dụng buff từ player
//         if (attacker != null && attacker.CompareTag("Player"))
//         {
//             realDuration *= (1f + attacker.burnDurationBonus);
//             realDamage *= (1f + attacker.burnDamageBonus);
//         }

//         if (sr != null)
//             sr.color = new Color(1f, 0.5f, 0.2f);

//         while (timer < realDuration)
//         {
//             if (health == null || health.currentHealth <= 0)
//                 break;

//             health.TakeDamage(realDamage * Time.deltaTime, false);

//             timer += Time.deltaTime;
//             yield return null;
//         }

//         if (sr != null)
//             sr.color = Color.white;

//         isBurning = false;
//     }
// }
using UnityEngine;
using System.Collections;

public class BurnEnemy : MonoBehaviour
{
    [Header("Fire Settings")]
    [SerializeField] private float burnDuration = 3f;
    [SerializeField] private float burnDamagePerSecond = 5f;

    private bool isBurning = false;
    private SpriteRenderer sr;
    private Health health;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
        health = GetComponent<Health>();
    }

    public void TriggerBurn(PlayerAttack attacker)
    {
        if (health != null && health.currentHealth <= 0) return;
        if (!isBurning)
            StartCoroutine(Burn(attacker));
    }

    private IEnumerator Burn(PlayerAttack attacker)
    {
        isBurning = true;
        float timer = 0f;

        float realDuration = burnDuration;
        float realDamage = burnDamagePerSecond;

        // ⭐ Chỉ áp dụng buff từ player
        if (attacker != null && attacker.CompareTag("Player"))
        {
            realDuration *= (1f + attacker.burnDurationBonus);
            realDamage *= (1f + attacker.burnDamageBonus);
        }

        // Debug.Log(
        //     $"🔥 START BURN on {gameObject.name} | " +
        //     $"DamagePerSec = {realDamage:F2}, Duration = {realDuration:F2}, Attacker = {(attacker ? "Player" : "Enemy")}"
        // );

        if (sr != null)
            sr.color = new Color(1f, 0.5f, 0.2f);

        float accumulatedDamage = 0f; // ⭐ Debug mỗi giây

        while (timer < realDuration)
        {
            if (health == null || health.currentHealth <= 0)
                break;

            float damageThisFrame = realDamage * Time.deltaTime;
            accumulatedDamage += damageThisFrame;

            health.TakeDamage(damageThisFrame, false);

            // Debug.Log(
            //     $"🔥 DOT HIT {gameObject.name}: +{damageThisFrame:F3} dmg | " +
            //     $"TotalSoFar={accumulatedDamage:F2} at t={timer:F2}s"
            // );

            // Debug mỗi giây
            if (Mathf.Floor(timer) != Mathf.Floor(timer + Time.deltaTime))
            {
                // Debug.Log(
                //     $"🔥 1-SECOND DOT SUMMARY on {gameObject.name}: {accumulatedDamage:F2} damage"
                // );
                accumulatedDamage = 0f;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        if (sr != null)
            sr.color = Color.white;

        // Debug.Log($"🔥 END BURN on {gameObject.name}");

        isBurning = false;
    }
}
