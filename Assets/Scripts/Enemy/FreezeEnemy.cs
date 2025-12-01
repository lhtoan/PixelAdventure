// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;

// public class FreezeEnemy : MonoBehaviour
// {
//     [Header("Movement Script (optional)")]
//     [SerializeField] private MonoBehaviour customMovementScript;

//     [Header("Ice Settings")]
//     [SerializeField] private int iceHitsToFreeze = 3;
//     [SerializeField] private float freezeDuration = 2f;
//     [SerializeField] private Transform iceStateParent;
//     [SerializeField] private float stackResetDelay = 2f;

//     private Animator anim;
//     private SpriteRenderer sr;
//     private Health health;
//     private bool isFrozen;
//     private int iceHitCount;
//     private GameObject[] iceStates;
//     private Coroutine resetCoroutine;

//     private Rigidbody2D rb;
//     private float originalGravity;

//     // All movement scripts that implement IEnemyMovement
//     private List<IEnemyMovement> movementScripts = new();

//     private void Awake()
//     {
//         anim = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
//         sr = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
//         health = GetComponent<Health>();
//         rb = GetComponent<Rigidbody2D>();

//         if (rb != null)
//             originalGravity = rb.gravityScale;

//         // Collect all movement scripts (on this object & children)
//         foreach (var comp in GetComponentsInChildren<MonoBehaviour>())
//         {
//             if (comp is IEnemyMovement movement)
//                 movementScripts.Add(movement);
//         }

//         // read ice state child objects
//         if (iceStateParent != null)
//         {
//             int count = iceStateParent.childCount;
//             iceStates = new GameObject[count];
//             for (int i = 0; i < count; i++)
//             {
//                 iceStates[i] = iceStateParent.GetChild(i).gameObject;
//                 iceStates[i].SetActive(false);
//             }
//         }
//     }

//     public void TriggerIceHit()
//     {
//         if (health != null && health.currentHealth <= 0) return;
//         if (isFrozen) return;

//         if (resetCoroutine != null)
//             StopCoroutine(resetCoroutine);

//         resetCoroutine = StartCoroutine(ResetStackAfterDelay());

//         iceHitCount++;
//         ShowIceState(iceHitCount);

//         if (iceHitCount >= iceHitsToFreeze)
//             TriggerFreeze(freezeDuration);
//     }

//     private IEnumerator ResetStackAfterDelay()
//     {
//         yield return new WaitForSeconds(stackResetDelay);

//         if (!isFrozen)
//         {
//             iceHitCount = 0;
//             HideAllIceStates();
//         }
//     }

//     public void TriggerFreeze(float duration)
//     {
//         if (isFrozen) return;
//         StartCoroutine(FreezeRoutine(duration));
//     }

//     private IEnumerator FreezeRoutine(float duration)
//     {
//         isFrozen = true;

//         // Stop rigidbody immediately (no sliding)
//         if (rb != null)
//         {
//             rb.linearVelocity = Vector2.zero;
//             rb.angularVelocity = 0f;
//             rb.gravityScale = 0f;
//             rb.constraints = RigidbodyConstraints2D.FreezePosition;
//         }

//         // Stop all movement scripts
//         foreach (var move in movementScripts)
//             move.SetFrozen(true);

//         if (customMovementScript != null)
//             customMovementScript.enabled = false;

//         // Freeze animation
//         if (anim != null)
//             anim.speed = 0;

//         // Show frozen state
//         if (iceStates != null && iceStates.Length > 0)
//         {
//             foreach (var g in iceStates) g.SetActive(false);
//             iceStates[iceStates.Length - 1].SetActive(true);
//         }

//         // Tint
//         if (sr != null)
//             sr.color = new Color(0.6f, 0.8f, 1f);

//         // Wait
//         float t = 0f;
//         while (t < duration)
//         {
//             // ⛔ Nếu chết khi đang đóng băng → thoát ngay
//             if (health != null && health.currentHealth <= 0)
//             {
//                 Unfreeze();
//                 yield break;
//             }

//             t += Time.deltaTime;
//             yield return null;
//         }


//         Unfreeze();
//     }

//     private void Unfreeze()
//     {
//         if (!isFrozen) return;

//         // Restore Rigidbody
//         if (rb != null)
//         {
//             rb.gravityScale = originalGravity;
//             rb.constraints = RigidbodyConstraints2D.FreezeRotation;
//         }

//         // Restore animation
//         if (anim != null)
//             anim.speed = 1;

//         // Restore color
//         if (sr != null)
//             sr.color = Color.white;

//         // Re-enable movement
//         foreach (var move in movementScripts)
//             move.SetFrozen(false);

//         if (customMovementScript != null)
//             customMovementScript.enabled = true;

//         // Reset states
//         iceHitCount = 0;
//         HideAllIceStates();
//         isFrozen = false;
//     }

//     private void ShowIceState(int count)
//     {
//         if (iceStates == null || iceStates.Length == 0) return;

//         foreach (var g in iceStates) g.SetActive(false);

//         int idx = Mathf.Clamp(count - 1, 0, iceStates.Length - 1);
//         iceStates[idx].SetActive(true);
//     }

//     private void HideAllIceStates()
//     {
//         if (iceStates == null) return;
//         foreach (var g in iceStates) g.SetActive(false);
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

    // ⭐ Tích lũy damage để nổ
    private float accumulatedIceDamage = 0f;


    private void Awake()
    {
        anim = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
        sr = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
            originalGravity = rb.gravityScale;

        movementScripts = new List<IEnemyMovement>();

        // ⭐ LẤY SCRIPT IEnemyMovement TỪ PARENT
        foreach (var comp in GetComponentsInParent<MonoBehaviour>())
        {
            if (comp is IEnemyMovement move)
                movementScripts.Add(move);
        }

        // ⭐ LẤY SCRIPT IEnemyMovement TỪ CHILDREN (nếu có)
        foreach (var comp in GetComponentsInChildren<MonoBehaviour>())
        {
            if (comp is IEnemyMovement move)
                movementScripts.Add(move);
        }

        // Ice State setup
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


    // ===============================================================
    //  ⭐ HÀM BỊ PROJECTILE GỌI KHI NHẬN DAMAGE BĂNG
    // ===============================================================
    // public void ApplyIceDamage(float damage, PlayerAttack attacker)
    // {
    //     if (health == null || health.currentHealth <= 0) return;

    //     // ⭐ Enemy đang đóng băng vẫn phải nhận damage
    //     if (isFrozen)
    //     {
    //         health.TakeDamage(damage, false);
    //         return;    // nhưng KHÔNG TÍCH STACK, KHÔNG NỔ
    //     }

    //     // ⭐ Gây damage bình thường
    //     health.TakeDamage(damage, false);

    //     // Kiểm tra skill IceStack
    //     PlayerSkill ps = attacker.GetComponent<PlayerSkill>();
    //     bool hasIceStack = ps != null && ps.IsSkillUnlocked(PlayerSkill.SkillType.IceStack);

    //     if (hasIceStack)
    //         accumulatedIceDamage += damage;

    //     // Tăng stack
    //     TriggerIceHit();

    //     // Đủ stack → Nổ
    //     if (iceHitCount >= iceHitsToFreeze && hasIceStack)
    //     {
    //         BurstIceDamage();
    //     }
    // }

    public void ApplyIceDamage(float damage, PlayerAttack attacker)
    {
        if (health == null || health.currentHealth <= 0) return;

        // luôn gây damage
        health.TakeDamage(damage, false);

        PlayerSkill ps = attacker.GetComponent<PlayerSkill>();
        bool hasIceStack = ps != null && ps.IsSkillUnlocked(PlayerSkill.SkillType.IceStack);

        // ❄ luôn cộng stack, kể cả khi đang frozen
        iceHitCount++;

        // nếu có IceStack → cộng accumulated để burst
        if (hasIceStack)
            accumulatedIceDamage += damage;

        // cập nhật visual stack
        ShowIceState(iceHitCount);

        // đủ 3 stack → xử lý
        if (iceHitCount >= iceHitsToFreeze)
        {
            // reset stack
            iceHitCount = 0;

            if (!isFrozen)
                TriggerFreeze(freezeDuration);    // chỉ freeze nếu chưa frozen

            if (hasIceStack)
                BurstIceDamage();               // nổ dame mỗi 3 stack
        }

        // reset stack delay
        if (resetCoroutine != null)
            StopCoroutine(resetCoroutine);
        resetCoroutine = StartCoroutine(ResetStackAfterDelay());
    }




    // ⭐ Nổ dame bằng tổng tích lũy
    private void BurstIceDamage()
    {
        if (accumulatedIceDamage <= 0f) return;

        Debug.Log($"❄ ICE BURST! Damage = {accumulatedIceDamage}");

        health.TakeDamage(accumulatedIceDamage, true);

        accumulatedIceDamage = 0f;
    }


    // ===============================================================

    public void TriggerIceHit()
    {
        if (health != null && health.currentHealth <= 0) return;
        if (isFrozen) return; // frozen không tăng stack

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
            accumulatedIceDamage = 0f;
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

        // Stop rigidbody immediately
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezePosition;
        }

        // Stop movement
        foreach (var move in movementScripts)
            move.SetFrozen(true);

        if (customMovementScript != null)
            customMovementScript.enabled = false;

        // Freeze animation
        if (anim != null)
            anim.speed = 0;

        // Show final frozen state
        if (iceStates != null && iceStates.Length > 0)
        {
            foreach (var g in iceStates) g.SetActive(false);
            iceStates[iceStates.Length - 1].SetActive(true);
        }

        if (sr != null)
            sr.color = new Color(0.6f, 0.8f, 1f);

        float t = 0f;
        while (t < duration)
        {
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

        if (rb != null)
        {
            rb.gravityScale = originalGravity;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        if (anim != null)
            anim.speed = 1;

        if (sr != null)
            sr.color = Color.white;

        foreach (var move in movementScripts)
            move.SetFrozen(false);

        if (customMovementScript != null)
            customMovementScript.enabled = true;

        iceHitCount = 0;
        accumulatedIceDamage = 0f;
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
