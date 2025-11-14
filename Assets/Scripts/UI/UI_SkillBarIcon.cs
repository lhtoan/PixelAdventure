using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_SkillBarIcon : MonoBehaviour
{
    [Header("Icon Children (must exist)")]
    public GameObject unlockedIcon;   // Icon màu
    public Image lockedFill;          // Icon xám chạy cooldown (radial)

    private Coroutine cooldownRoutine;

    private void Awake()
    {
        // Auto find if not assigned
        if (unlockedIcon == null)
        {
            Transform u = transform.Find("unlocked");
            if (u != null) unlockedIcon = u.gameObject;
        }

        if (lockedFill == null)
        {
            Transform l = transform.Find("locked");
            if (l != null) lockedFill = l.GetComponent<Image>();
        }

        if (lockedFill != null)
        {
            lockedFill.fillAmount = 0f;
            lockedFill.gameObject.SetActive(false);
        }

        // ❗Không được tắt GameObject này — UI_SkillBar chỉ tắt unlockedIcon
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Bắt đầu cooldown UI icon.
    /// </summary>
    public void StartCooldown(float cooldown)
    {
        // Nếu icon bị tắt vì một lý do nào đó thì bật lên để coroutine chạy được
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        if (lockedFill == null)
        {
            Debug.LogError("lockedFill is NULL in " + name);
            return;
        }

        // Reset coroutine
        if (cooldownRoutine != null)
            StopCoroutine(cooldownRoutine);

        cooldownRoutine = StartCoroutine(CooldownRoutine(cooldown));
    }

    private IEnumerator CooldownRoutine(float cd)
    {
        // Bật overlay xám
        lockedFill.gameObject.SetActive(true);
        lockedFill.fillAmount = 1f;

        float t = cd;

        while (t > 0f)
        {
            t -= Time.deltaTime;
            lockedFill.fillAmount = Mathf.Clamp01(t / cd);
            yield return null;
        }

        // Cooldown xong
        lockedFill.fillAmount = 0f;
        lockedFill.gameObject.SetActive(false);

        cooldownRoutine = null;
    }
}
