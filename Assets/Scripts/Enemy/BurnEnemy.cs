using UnityEngine;
using System.Collections;

public class BurnEnemy : MonoBehaviour
{
    [Header("Fire Settings")]
    [SerializeField] private float burnDuration = 3f;    // thời gian cháy
    [SerializeField] private float burnDamagePerSecond = 5f; // sát thương mỗi giây

    private bool isBurning = false;
    private SpriteRenderer sr;
    private Health health;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
        health = GetComponent<Health>();
    }

    // 🔥 Gọi khi trúng Fireball
    public void TriggerBurn()
    {
        if (health != null && health.currentHealth <= 0) return;
        if (!isBurning)
            StartCoroutine(Burn());
    }

    private IEnumerator Burn()
    {
        isBurning = true;
        float timer = 0f;

        if (sr != null)
            sr.color = new Color(1f, 0.5f, 0.2f);

        while (timer < burnDuration)
        {
            if (health == null || health.currentHealth <= 0)
                break;

            // Gây sát thương mà KHÔNG bật animation “hurt”
            health.TakeDamage(burnDamagePerSecond * Time.deltaTime, false);

            timer += Time.deltaTime;
            yield return null;
        }

        if (sr != null)
            sr.color = Color.white;

        isBurning = false;
    }

}
