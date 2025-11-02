using UnityEngine;
using System.Collections;

public class FreezeEnemy : MonoBehaviour
{
    [Header("Tùy chọn: Gán script di chuyển (nếu không dùng EnemyPatrol)")]
    [SerializeField] private MonoBehaviour customMovementScript; // Kéo thả script di chuyển vào nếu không dùng EnemyPatrol

    private Animator anim;
    private EnemyPatrol patrolScript;
    private SpriteRenderer sr;
    private Health health;
    private bool isFrozen;

    private void Awake()
    {
        anim = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
        sr = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
        health = GetComponent<Health>();

        // ✅ Vẫn giữ hệ thống tự động tìm EnemyPatrol như cũ
        patrolScript = GetComponentInParent<EnemyPatrol>();
    }

    public void TriggerFreeze(float duration)
    {
        if (health != null && health.currentHealth <= 0) return;
        if (isFrozen) return;

        StartCoroutine(Freeze(duration));
    }

    private IEnumerator Freeze(float duration)
    {
        isFrozen = true;

        // Nếu enemy đã chết ngay lúc bắt đầu
        if (health != null && health.currentHealth <= 0)
        {
            isFrozen = false;
            yield break;
        }

        // ❄️ Dừng animation
        if (anim != null)
            anim.speed = 0;

        // ❄️ Dừng di chuyển
        if (patrolScript != null)
            patrolScript.isFrozen = true;
        else if (customMovementScript != null)
            customMovementScript.enabled = false;

        // ❄️ Đổi màu enemy
        if (sr != null)
            sr.color = new Color(0.6f, 0.8f, 1f);

        float timer = 0f;
        while (timer < duration)
        {
            // Nếu enemy chết trong lúc đóng băng → hủy hiệu ứng luôn
            if (health != null && health.currentHealth <= 0)
            {
                Unfreeze();
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // 🔓 Rã băng
        Unfreeze();
    }

    private void Unfreeze()
    {
        if (anim != null)
            anim.speed = 1;

        if (patrolScript != null)
            patrolScript.isFrozen = false;
        else if (customMovementScript != null)
            customMovementScript.enabled = true;

        if (sr != null)
            sr.color = Color.white;

        isFrozen = false;
    }
}
