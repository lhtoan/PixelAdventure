using UnityEngine;
using System.Collections;

public class FreezeEnemy : MonoBehaviour
{
    [Header("Tùy chọn: Gán script di chuyển (nếu không dùng EnemyPatrol)")]
    [SerializeField] private MonoBehaviour customMovementScript;

    [Header("Iceball Settings")]
    [SerializeField] private int iceHitsToFreeze = 3;
    [SerializeField] private float freezeDuration = 2f;
    [SerializeField] private Transform iceStateParent;
    [SerializeField] private float fadeDuration = 0.2f;
    [SerializeField] private float stackResetDelay = 2f; // ⏳ Thời gian tự reset stack

    private Animator anim;
    private EnemyPatrol patrolScript;
    private SpriteRenderer sr;
    private Health health;
    private bool isFrozen;
    private int iceHitCount = 0;
    private GameObject[] iceStates;

    private Coroutine resetCoroutine; // ⏱ Đếm ngược reset stack

    private void Awake()
    {
        anim = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
        sr = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
        health = GetComponent<Health>();
        patrolScript = GetComponentInParent<EnemyPatrol>();

        // Lấy tất cả các state con trong IceState
        if (iceStateParent != null)
        {
            iceStates = new GameObject[iceStateParent.childCount];
            for (int i = 0; i < iceStateParent.childCount; i++)
            {
                iceStates[i] = iceStateParent.GetChild(i).gameObject;
                iceStates[i].SetActive(false);
            }
        }
    }

    public void TriggerIceHit()
    {
        if (health != null && health.currentHealth <= 0) return;

        // Mỗi lần trúng đòn → reset bộ đếm reset stack
        if (resetCoroutine != null)
            StopCoroutine(resetCoroutine);
        resetCoroutine = StartCoroutine(ResetStackAfterDelay());

        iceHitCount++;
        // Debug.Log($"{name} Ice stack: {iceHitCount}/{iceHitsToFreeze}");

        ShowIceState(iceHitCount);

        if (iceHitCount >= iceHitsToFreeze)
        {
            TriggerFreeze(freezeDuration);
            iceHitCount = 1;
        }
    }

    private IEnumerator ResetStackAfterDelay()
    {
        yield return new WaitForSeconds(stackResetDelay);

        // Sau khi chờ xong mà chưa đủ đòn → reset
        if (!isFrozen && iceHitCount > 0)
        {
            iceHitCount = 0;
            // Debug.Log($"{name} ❄️ Ice stack reset (không trúng thêm sau {stackResetDelay}s)");
            if (iceStates != null)
            {
                foreach (var s in iceStates)
                    StartCoroutine(FadeOutState(s));
            }
        }
    }

    private void ShowIceState(int count)
    {
        if (iceStates == null || iceStates.Length == 0) return;

        for (int i = 0; i < iceStates.Length; i++)
            StartCoroutine(FadeOutState(iceStates[i]));

        int index = Mathf.Clamp(count - 1, 0, iceStates.Length - 1);
        StartCoroutine(FadeInState(iceStates[index]));
    }

    private IEnumerator FadeInState(GameObject state)
    {
        state.SetActive(true);
        SpriteRenderer s = state.GetComponent<SpriteRenderer>();
        if (s == null) yield break;

        Color c = s.color;
        c.a = 0;
        s.color = c;

        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0, 1, t / fadeDuration);
            s.color = c;
            yield return null;
        }
        s.color = new Color(c.r, c.g, c.b, 1);
    }

    private IEnumerator FadeOutState(GameObject state)
    {
        if (!state.activeSelf) yield break;

        SpriteRenderer s = state.GetComponent<SpriteRenderer>();
        if (s == null) { state.SetActive(false); yield break; }

        Color c = s.color;
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1, 0, t / fadeDuration);
            s.color = c;
            yield return null;
        }
        state.SetActive(false);
    }

    public void TriggerFreeze(float duration)
    {
        if (health != null && health.currentHealth <= 0) return;
        if (isFrozen) return;

        // Ngừng reset stack khi đã đóng băng
        if (resetCoroutine != null)
            StopCoroutine(resetCoroutine);

        StartCoroutine(Freeze(duration));
    }

    private IEnumerator Freeze(float duration)
    {
        isFrozen = true;

        if (anim != null) anim.speed = 0;
        if (patrolScript != null) patrolScript.isFrozen = true;
        else if (customMovementScript != null) customMovementScript.enabled = false;

        if (sr != null) sr.color = new Color(0.6f, 0.8f, 1f);

        float timer = 0f;
        while (timer < duration)
        {
            if (health != null && health.currentHealth <= 0)
            {
                Unfreeze();
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        Unfreeze();
    }

    private void Unfreeze()
    {
        if (anim != null) anim.speed = 1;
        if (patrolScript != null) patrolScript.isFrozen = false;
        else if (customMovementScript != null) customMovementScript.enabled = true;

        if (sr != null) sr.color = Color.white;
        isFrozen = false;

        if (iceStates != null)
        {
            foreach (var s in iceStates)
                StartCoroutine(FadeOutState(s));
        }

        // Reset toàn bộ stack khi rã băng
        iceHitCount = 0;
    }
}
