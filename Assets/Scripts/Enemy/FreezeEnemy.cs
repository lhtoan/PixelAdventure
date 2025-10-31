using UnityEngine;
using System.Collections;

public class FreezeEnemy : MonoBehaviour
{
    private Animator anim;
    private EnemyPatrol patrolScript;
    private bool isFrozen;
    private SpriteRenderer sr;
    private Health health; // dùng hệ Health sẵn có của bạn

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        health = GetComponent<Health>();

        if (anim == null)
            anim = GetComponentInChildren<Animator>();
        if (sr == null)
            sr = GetComponentInChildren<SpriteRenderer>();

        // ✅ tìm EnemyPatrol ở object cha (vì thường nó gắn ở cha)
        patrolScript = GetComponentInParent<EnemyPatrol>();
    }

    public void TriggerFreeze(float duration)
    {
        // 🧱 Nếu enemy đã chết thì không làm gì cả
        if (health != null && health.currentHealth <= 0)
        {
            return;
        }

        if (isFrozen) return;
        StartCoroutine(Freeze(duration));
    }

    private IEnumerator Freeze(float duration)
    {
        isFrozen = true;

        // ❄️ Nếu enemy đã chết giữa lúc bắt đầu đóng băng → dừng luôn
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
        else
        {
            EnemyPatrol childPatrol = GetComponentInChildren<EnemyPatrol>();
            if (childPatrol != null)
                childPatrol.isFrozen = true;
        }

        // ❄️ Đổi màu enemy
        if (sr != null)
            sr.color = new Color(0.6f, 0.8f, 1f);

        // ⏳ Chờ thời gian đóng băng
        float timer = 0f;
        while (timer < duration)
        {
            // Nếu enemy chết trong lúc đang đóng băng → dừng ngay lập tức
            if (health != null && health.currentHealth <= 0)
            {

                // 🔓 Khôi phục tạm thời để script Health xử lý việc Destroy
                if (anim != null)
                    anim.speed = 1;

                if (patrolScript != null)
                    patrolScript.isFrozen = false;
                else
                {
                    EnemyPatrol childPatrol = GetComponentInChildren<EnemyPatrol>();
                    if (childPatrol != null)
                        childPatrol.isFrozen = false;
                }

                if (sr != null)
                    sr.color = Color.white;

                isFrozen = false;
                yield break;
            }


            timer += Time.deltaTime;
            yield return null;
        }

        // 🔓 Hết thời gian đóng băng → rã băng
        if (anim != null)
            anim.speed = 1;

        if (patrolScript != null)
            patrolScript.isFrozen = false;
        else
        {
            EnemyPatrol childPatrol = GetComponentInChildren<EnemyPatrol>();
            if (childPatrol != null)
                childPatrol.isFrozen = false;
        }

        if (sr != null)
            sr.color = Color.white;

        isFrozen = false;
    }
}
