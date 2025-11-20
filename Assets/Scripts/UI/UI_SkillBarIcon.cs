// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections;

// public class UI_SkillBarIcon : MonoBehaviour
// {
//     [Header("Icon Children (must exist)")]
//     public GameObject unlockedIcon;   // Icon màu
//     public Image lockedFill;          // Icon xám chạy cooldown (radial)

//     private Coroutine cooldownRoutine;

//     private void Awake()
//     {
//         // Auto find if not assigned
//         if (unlockedIcon == null)
//         {
//             Transform u = transform.Find("unlocked");
//             if (u != null) unlockedIcon = u.gameObject;
//         }

//         if (lockedFill == null)
//         {
//             Transform l = transform.Find("locked");
//             if (l != null) lockedFill = l.GetComponent<Image>();
//         }

//         if (lockedFill != null)
//         {
//             lockedFill.fillAmount = 0f;
//             lockedFill.gameObject.SetActive(false);
//         }

//         // ❗Không được tắt GameObject này — UI_SkillBar chỉ tắt unlockedIcon
//         gameObject.SetActive(true);
//     }

//     /// <summary>
//     /// Bắt đầu cooldown UI icon.
//     /// </summary>
//     public void StartCooldown(float cooldown)
//     {
//         // Nếu icon bị tắt vì một lý do nào đó thì bật lên để coroutine chạy được
//         if (!gameObject.activeSelf)
//             gameObject.SetActive(true);

//         if (lockedFill == null)
//         {
//             Debug.LogError("lockedFill is NULL in " + name);
//             return;
//         }

//         // Reset coroutine
//         if (cooldownRoutine != null)
//             StopCoroutine(cooldownRoutine);

//         cooldownRoutine = StartCoroutine(CooldownRoutine(cooldown));
//     }

//     private IEnumerator CooldownRoutine(float cd)
//     {
//         // Bật overlay xám
//         lockedFill.gameObject.SetActive(true);
//         lockedFill.fillAmount = 1f;

//         float t = cd;

//         while (t > 0f)
//         {
//             t -= Time.deltaTime;
//             lockedFill.fillAmount = Mathf.Clamp01(t / cd);
//             yield return null;
//         }

//         // Cooldown xong
//         lockedFill.fillAmount = 0f;
//         lockedFill.gameObject.SetActive(false);

//         cooldownRoutine = null;
//     }
// }
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_SkillBarIcon : MonoBehaviour
{
    [Header("Icon Children (must exist)")]
    public GameObject unlockedIcon;
    public GameObject greyIcon;
    public Image lockedFill;

    public float currentCooldownFill = 0f; // ⭐ NEW

    private Coroutine cooldownRoutine;

    private void Awake()
    {
        if (unlockedIcon == null)
            unlockedIcon = transform.Find("unlocked")?.gameObject;

        if (greyIcon == null)
            greyIcon = transform.Find("grey")?.gameObject;

        if (lockedFill == null)
            lockedFill = transform.Find("locked")?.GetComponent<Image>();

        if (greyIcon != null) greyIcon.SetActive(false);
        if (lockedFill != null)
        {
            lockedFill.fillAmount = 0f;
            lockedFill.gameObject.SetActive(false);
        }
    }

    public void StartCooldown(float cooldown)
    {
        if (cooldownRoutine != null)
            StopCoroutine(cooldownRoutine);

        cooldownRoutine = StartCoroutine(CooldownRoutine(cooldown));
    }

    private IEnumerator CooldownRoutine(float cd)
    {
        lockedFill.gameObject.SetActive(true);
        lockedFill.fillAmount = 1f;
        currentCooldownFill = 1f;

        float t = cd;

        while (t > 0f)
        {
            t -= Time.deltaTime;
            currentCooldownFill = Mathf.Clamp01(t / cd);
            lockedFill.fillAmount = currentCooldownFill;
            yield return null;
        }

        currentCooldownFill = 0f;
        lockedFill.fillAmount = 0f;
        lockedFill.gameObject.SetActive(false);
        cooldownRoutine = null;
    }
}
