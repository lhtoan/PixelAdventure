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
    private PlayerAttack playerAttack;
    private Dictionary<GameObject, float> enemyTickTimers = new Dictionary<GameObject, float>();

    private void Awake()
    {
        playerStamina = GetComponentInParent<PlayerStamina>();
        playerAttack = GetComponentInParent<PlayerAttack>();

        if (shieldObject != null)
            shieldObject.SetActive(false);
    }

    private void Update()
    {
        // ✅ Luôn lắng nghe phím, nhưng chỉ kích hoạt được khi hệ là Ice
        if (Input.GetKeyDown(KeyCode.E) && playerAttack.CurrentElement == PlayerAttack.Element.Ice)
        {
            TryActivateShield();
        }
    }

    private void TryActivateShield()
    {
        if (isActive) return;

        // ✅ Chỉ cho phép dùng nếu đang ở hệ Ice
        if (playerAttack == null)
        {
            Debug.Log("❌ Không thể bật Ice Shield khi không ở hệ Ice!");
            return;
        }

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

        // Trừ stamina và bật shield
        playerStamina.Use(staminaCost);
        StartCoroutine(ActivateShield());
    }

    private IEnumerator ActivateShield()
    {
        isActive = true;
        enemyTickTimers.Clear();

        if (shieldObject != null)
            shieldObject.SetActive(true);

        Debug.Log($"🧊 Ice Shield bật! (Tốn {staminaCost} stamina)");

        // ✅ Miễn sát thương cho player
        Health playerHealth = GetComponentInParent<Health>();
        if (playerHealth != null)
            playerHealth.SetShieldProtection(true);

        yield return new WaitForSeconds(duration);

        // ❌ Hết thời gian → tắt shield
        if (shieldObject != null)
            shieldObject.SetActive(false);

        if (playerHealth != null)
            playerHealth.SetShieldProtection(false);

        isActive = false;
        Debug.Log("🧊 Ice Shield tắt!");
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!isActive || shieldObject == null || !shieldObject.activeSelf) return;
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
