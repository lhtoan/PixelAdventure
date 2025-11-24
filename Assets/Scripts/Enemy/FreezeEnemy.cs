// using UnityEngine;
// using System.Collections;

// public class FreezeEnemy : MonoBehaviour
// {
//     [Header("Tùy chọn: Gán script di chuyển (nếu không dùng EnemyPatrol)")]
//     [SerializeField] private MonoBehaviour customMovementScript;

//     [Header("Iceball Settings")]
//     [SerializeField] private int iceHitsToFreeze = 3;
//     [SerializeField] private float freezeDuration = 2f;
//     [SerializeField] private Transform iceStateParent;
//     // [SerializeField] private float fadeDuration = 0.2f;
//     [SerializeField] private float stackResetDelay = 2f; // ⏳ Thời gian tự reset stack

//     private Animator anim;
//     private EnemyPatrol patrolScript;
//     private SpriteRenderer sr;
//     private Health health;
//     private bool isFrozen;
//     private int iceHitCount = 0;
//     private GameObject[] iceStates;

//     private Coroutine resetCoroutine; // ⏱ Đếm ngược reset stack

//     private void Awake()
//     {
//         anim = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
//         sr = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
//         health = GetComponent<Health>();
//         patrolScript = GetComponentInParent<EnemyPatrol>();

//         // Lấy tất cả các state con trong IceState
//         if (iceStateParent != null)
//         {
//             iceStates = new GameObject[iceStateParent.childCount];
//             for (int i = 0; i < iceStateParent.childCount; i++)
//             {
//                 iceStates[i] = iceStateParent.GetChild(i).gameObject;
//                 iceStates[i].SetActive(false);
//             }
//         }
//     }

//     public void TriggerIceHit()
//     {
//         if (health != null && health.currentHealth <= 0) return;
//         if (isFrozen) return; // 🔒 Khi đã bị đóng băng thì không cộng stack nữa

//         // Reset timer reset stack mỗi khi bị trúng đòn
//         if (resetCoroutine != null)
//             StopCoroutine(resetCoroutine);
//         resetCoroutine = StartCoroutine(ResetStackAfterDelay());

//         iceHitCount++;
//         ShowIceState(iceHitCount);

//         if (iceHitCount >= iceHitsToFreeze)
//         {
//             TriggerFreeze(freezeDuration);
//             // ❌ KHÔNG reset stack ở đây — sẽ reset khi rã băng
//         }
//     }

//     private IEnumerator ResetStackAfterDelay()
//     {
//         yield return new WaitForSeconds(stackResetDelay);

//         // Nếu chưa bị đóng băng mà không trúng thêm → reset stack
//         if (!isFrozen && iceHitCount > 0)
//         {
//             iceHitCount = 0;
//             HideAllIceStates();
//         }
//     }

//     private void ShowIceState(int count)
//     {
//         if (iceStates == null || iceStates.Length == 0) return;

//         // Ẩn toàn bộ, rồi hiển thị mức stack hiện tại
//         for (int i = 0; i < iceStates.Length; i++)
//             iceStates[i].SetActive(false);

//         int index = Mathf.Clamp(count - 1, 0, iceStates.Length - 1);
//         iceStates[index].SetActive(true);
//     }

//     private void HideAllIceStates()
//     {
//         if (iceStates == null) return;

//         foreach (var s in iceStates)
//             s.SetActive(false);
//     }

//     public void TriggerFreeze(float duration)
//     {
//         if (isFrozen) return;
//         if (health != null && health.currentHealth <= 0) return;

//         if (resetCoroutine != null)
//             StopCoroutine(resetCoroutine);

//         StartCoroutine(Freeze(duration));
//     }

//     private IEnumerator Freeze(float duration)
//     {
//         isFrozen = true;

//         var rb = GetComponent<Rigidbody2D>();
//         if (rb != null)
//         {
//             rb.linearVelocity = Vector2.zero;
//             rb.angularVelocity = 0f;

//             rb.gravityScale = 0f; // ❗ Tắt gravity khi đóng băng
//             rb.constraints = RigidbodyConstraints2D.FreezePosition;
//         }


//         // Hiển thị trạng thái "băng đầy" khi đóng băng
//         if (iceStates != null && iceStates.Length > 0)
//         {
//             foreach (var s in iceStates)
//                 s.SetActive(false);
//             iceStates[iceStates.Length - 1].SetActive(true);
//         }

//         if (anim != null) anim.speed = 0;
//         if (patrolScript != null) patrolScript.isFrozen = true;
//         else if (customMovementScript != null) customMovementScript.enabled = false;

//         if (sr != null) sr.color = new Color(0.6f, 0.8f, 1f);

//         float timer = 0f;
//         while (timer < duration)
//         {
//             if (health != null && health.currentHealth <= 0)
//             {
//                 Unfreeze();
//                 yield break;
//             }

//             timer += Time.deltaTime;
//             yield return null;
//         }

//         Unfreeze();
//     }

//     private void Unfreeze()
//     {
//         var rb = GetComponent<Rigidbody2D>();
//         if (rb != null)
//         {
//             rb.gravityScale = 4f; // ❗ Bật lại gravity (đúng giá trị cũ của bạn)
//             rb.constraints = RigidbodyConstraints2D.FreezeRotation;
//         }



//         if (anim != null) anim.speed = 1;
//         if (patrolScript != null) patrolScript.isFrozen = false;
//         else if (customMovementScript != null) customMovementScript.enabled = true;

//         if (sr != null) sr.color = Color.white;

//         isFrozen = false;
//         iceHitCount = 0; // ✅ Reset stack khi rã băng
//         HideAllIceStates(); // ✅ Ẩn hết hiệu ứng băng
//     }
// }
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FreezeEnemy : MonoBehaviour
{
    [Header("Movement Script (optional)")]
    [SerializeField] private MonoBehaviour customMovementScript;

    [Header("Ice Settings")]
    [SerializeField] private int iceHitsToFreeze = 3;
    [SerializeField] private float freezeDuration = 2f;
    [SerializeField] private Transform iceStateParent;
    [SerializeField] private float stackResetDelay = 2f;

    private Animator anim;
    private SpriteRenderer sr;
    private Health health;
    private bool isFrozen;
    private int iceHitCount;
    private GameObject[] iceStates;
    private Coroutine resetCoroutine;

    private Rigidbody2D rb;
    private float originalGravity;

    // All movement scripts that implement IEnemyMovement
    private List<IEnemyMovement> movementScripts = new();

    private void Awake()
    {
        anim = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
        sr = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
            originalGravity = rb.gravityScale;

        // Collect all movement scripts (on this object & children)
        foreach (var comp in GetComponentsInChildren<MonoBehaviour>())
        {
            if (comp is IEnemyMovement movement)
                movementScripts.Add(movement);
        }

        // read ice state child objects
        if (iceStateParent != null)
        {
            int count = iceStateParent.childCount;
            iceStates = new GameObject[count];
            for (int i = 0; i < count; i++)
            {
                iceStates[i] = iceStateParent.GetChild(i).gameObject;
                iceStates[i].SetActive(false);
            }
        }
    }

    public void TriggerIceHit()
    {
        if (health != null && health.currentHealth <= 0) return;
        if (isFrozen) return;

        if (resetCoroutine != null)
            StopCoroutine(resetCoroutine);

        resetCoroutine = StartCoroutine(ResetStackAfterDelay());

        iceHitCount++;
        ShowIceState(iceHitCount);

        if (iceHitCount >= iceHitsToFreeze)
            TriggerFreeze(freezeDuration);
    }

    private IEnumerator ResetStackAfterDelay()
    {
        yield return new WaitForSeconds(stackResetDelay);

        if (!isFrozen)
        {
            iceHitCount = 0;
            HideAllIceStates();
        }
    }

    public void TriggerFreeze(float duration)
    {
        if (isFrozen) return;
        StartCoroutine(FreezeRoutine(duration));
    }

    private IEnumerator FreezeRoutine(float duration)
    {
        isFrozen = true;

        // Stop rigidbody immediately (no sliding)
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezePosition;
        }

        // Stop all movement scripts
        foreach (var move in movementScripts)
            move.SetFrozen(true);

        if (customMovementScript != null)
            customMovementScript.enabled = false;

        // Freeze animation
        if (anim != null)
            anim.speed = 0;

        // Show frozen state
        if (iceStates != null && iceStates.Length > 0)
        {
            foreach (var g in iceStates) g.SetActive(false);
            iceStates[iceStates.Length - 1].SetActive(true);
        }

        // Tint
        if (sr != null)
            sr.color = new Color(0.6f, 0.8f, 1f);

        // Wait
        float t = 0f;
        while (t < duration)
        {
            // ⛔ Nếu chết khi đang đóng băng → thoát ngay
            if (health != null && health.currentHealth <= 0)
            {
                Unfreeze();
                yield break;
            }

            t += Time.deltaTime;
            yield return null;
        }


        Unfreeze();
    }

    private void Unfreeze()
    {
        if (!isFrozen) return;

        // Restore Rigidbody
        if (rb != null)
        {
            rb.gravityScale = originalGravity;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        // Restore animation
        if (anim != null)
            anim.speed = 1;

        // Restore color
        if (sr != null)
            sr.color = Color.white;

        // Re-enable movement
        foreach (var move in movementScripts)
            move.SetFrozen(false);

        if (customMovementScript != null)
            customMovementScript.enabled = true;

        // Reset states
        iceHitCount = 0;
        HideAllIceStates();
        isFrozen = false;
    }

    private void ShowIceState(int count)
    {
        if (iceStates == null || iceStates.Length == 0) return;

        foreach (var g in iceStates) g.SetActive(false);

        int idx = Mathf.Clamp(count - 1, 0, iceStates.Length - 1);
        iceStates[idx].SetActive(true);
    }

    private void HideAllIceStates()
    {
        if (iceStates == null) return;
        foreach (var g in iceStates) g.SetActive(false);
    }
}
