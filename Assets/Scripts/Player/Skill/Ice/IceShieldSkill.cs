using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IceShieldSkill : MonoBehaviour
{
    [Header("Shield Settings")]
    [SerializeField] private float duration = 5f;
    [SerializeField] private float staminaCost = 4f;
    [SerializeField] private float damagePerTick = 1f;
    [SerializeField] private float tickInterval = 2f;
    [SerializeField] private GameObject shieldObject;

    private bool isActive = false;
    private PlayerStamina playerStamina;
    private Dictionary<GameObject, float> enemyTickTimers = new Dictionary<GameObject, float>();

    private void Awake()
    {
        playerStamina = GetComponentInParent<PlayerStamina>();

        if (shieldObject != null)
            shieldObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryActivateShield();
        }
    }

    private void TryActivateShield()
    {
        if (isActive) return;

        if (playerStamina == null)
        {
            Debug.LogWarning("⚠ Không tìm thấy PlayerStamina!");
            return;
        }

        if (!playerStamina.CanUse(staminaCost))
        {
            Debug.Log("❌ Không đủ stamina để bật Shield!");
            return;
        }

        playerStamina.Use(staminaCost);
        StartCoroutine(ActivateShield());
    }

    private IEnumerator ActivateShield()
    {
        isActive = true;
        enemyTickTimers.Clear();

        if (shieldObject != null)
            shieldObject.SetActive(true);

        Debug.Log("🧊 Shield bật! (Tốn " + staminaCost + " stamina)");

        yield return new WaitForSeconds(duration);

        if (shieldObject != null)
            shieldObject.SetActive(false);

        isActive = false;
        Debug.Log("🧊 Shield tắt!");
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!isActive || !shieldObject.activeSelf) return;
        if (!collision.gameObject.CompareTag("Enemy")) return;

        GameObject enemy = collision.gameObject;

        if (!enemyTickTimers.ContainsKey(enemy))
            enemyTickTimers[enemy] = 0f;

        enemyTickTimers[enemy] -= Time.deltaTime;

        if (enemyTickTimers[enemy] <= 0f)
        {
            ApplyIceEffect(enemy);
            enemyTickTimers[enemy] = tickInterval;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (enemyTickTimers.ContainsKey(collision.gameObject))
            enemyTickTimers.Remove(collision.gameObject);
    }

    private void ApplyIceEffect(GameObject enemy)
    {
        Health health = enemy.GetComponent<Health>();
        if (health != null)
            health.TakeDamage(damagePerTick);

        FreezeEnemy freeze = enemy.GetComponent<FreezeEnemy>();
        if (freeze != null)
            freeze.TriggerIceHit();

        Debug.Log($"🧊 Shield gây {damagePerTick} damage + 1 stack lên {enemy.name}");
    }
}
