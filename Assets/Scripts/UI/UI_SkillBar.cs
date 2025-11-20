using UnityEngine;
using System.Collections;

public class UI_SkillBar : MonoBehaviour
{
    [Header("Skill Icons")]
    public GameObject fireE_Icon;
    public GameObject fireR_Icon;
    public GameObject iceE_Icon;
    public GameObject iceR_Icon;

    private PlayerSkill playerSkill;

    private void Start()
    {
        playerSkill = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSkill>();

        // 🔥 Tắt toàn bộ icon con khi bắt đầu game (skill bar trống)
        InitEmptyState(fireE_Icon);
        InitEmptyState(fireR_Icon);
        InitEmptyState(iceE_Icon);
        InitEmptyState(iceR_Icon);

        RefreshSkillBar();
        PlayerAttack.Element current = FindFirstObjectByType<PlayerAttack>().CurrentElement;
        UpdateElementUI(current);
    }

    private void InitEmptyState(GameObject iconObj)
    {
        UI_SkillBarIcon icon = iconObj.GetComponent<UI_SkillBarIcon>();
        if (icon == null) return;

        icon.unlockedIcon.SetActive(false);
        icon.lockedFill.gameObject.SetActive(false);
    }

    public void RefreshSkillBar()
    {
        UpdateUnlockState(fireE_Icon, PlayerSkill.SkillType.Fire_E);
        UpdateUnlockState(fireR_Icon, PlayerSkill.SkillType.Fire_R);
        UpdateUnlockState(iceE_Icon, PlayerSkill.SkillType.Ice_E);
        UpdateUnlockState(iceR_Icon, PlayerSkill.SkillType.Ice_R);
    }

    private void UpdateUnlockState(GameObject iconObj, PlayerSkill.SkillType type)
    {
        UI_SkillBarIcon icon = iconObj.GetComponent<UI_SkillBarIcon>();
        if (icon == null) return;

        bool unlocked = playerSkill.IsSkillUnlocked(type);

        if (!unlocked)
        {
            icon.unlockedIcon.SetActive(false);
            icon.lockedFill.gameObject.SetActive(false);
            return;
        }

        // Nếu mới unlock → hiện icon + animation
        if (!icon.unlockedIcon.activeSelf)
        {
            icon.unlockedIcon.SetActive(true);
            StartCoroutine(UnlockAnimation(icon.unlockedIcon.transform));
        }
    }

    private IEnumerator UnlockAnimation(Transform target)
    {
        float time = 0.15f;

        Vector3 small = Vector3.one * 0.6f;
        Vector3 big = Vector3.one * 1.2f;
        Vector3 normal = Vector3.one;

        CanvasGroup cg = target.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = target.gameObject.AddComponent<CanvasGroup>();

        cg.alpha = 0f;
        target.localScale = small;

        float t = 0f;
        while (t < time)
        {
            t += Time.unscaledDeltaTime * 1.5f;
            target.localScale = Vector3.Lerp(small, big, t / time);
            cg.alpha = t / time;
            yield return null;
        }

        t = 0f;
        while (t < time)
        {
            t += Time.unscaledDeltaTime * 2f;
            target.localScale = Vector3.Lerp(big, normal, t / time);
            yield return null;
        }

        target.localScale = normal;
        cg.alpha = 1f;
    }

    public void UpdateElementUI(PlayerAttack.Element currentElement)
    {
        UpdateOneIcon(fireE_Icon,
            currentElement == PlayerAttack.Element.Fire &&
            playerSkill.IsSkillUnlocked(PlayerSkill.SkillType.Fire_E));

        UpdateOneIcon(fireR_Icon,
            currentElement == PlayerAttack.Element.Fire &&
            playerSkill.IsSkillUnlocked(PlayerSkill.SkillType.Fire_R));

        UpdateOneIcon(iceE_Icon,
            currentElement == PlayerAttack.Element.Ice &&
            playerSkill.IsSkillUnlocked(PlayerSkill.SkillType.Ice_E));

        UpdateOneIcon(iceR_Icon,
            currentElement == PlayerAttack.Element.Ice &&
            playerSkill.IsSkillUnlocked(PlayerSkill.SkillType.Ice_R));
    }

    // private void UpdateOneIcon(GameObject iconObj, bool active)
    // {
    //     UI_SkillBarIcon icon = iconObj.GetComponent<UI_SkillBarIcon>();
    //     if (icon == null) return;

    //     icon.unlockedIcon.SetActive(active);
    //     icon.lockedFill.gameObject.SetActive(!active);

    //     if (!active)
    //         icon.lockedFill.fillAmount = 1f;
    // }
    private void UpdateOneIcon(GameObject iconObj, bool active)
    {
        UI_SkillBarIcon icon = iconObj.GetComponent<UI_SkillBarIcon>();
        if (icon == null) return;

        // hiển thị icon đúng hệ
        icon.unlockedIcon.SetActive(active);

        // hiển thị grey sai hệ
        icon.greyIcon.SetActive(!active);

        // xử lý cooldown
        if (!active)
        {
            // sai hệ → chỉ hiện cooldown nếu đang chạy
            bool hasCooldown = icon.currentCooldownFill > 0f;
            icon.lockedFill.gameObject.SetActive(hasCooldown);
            if (hasCooldown)
                icon.lockedFill.fillAmount = icon.currentCooldownFill;
        }
        else
        {
            // đúng hệ → nếu cooldown đang chạy thì overlay phải bật
            bool hasCooldown = icon.currentCooldownFill > 0f;
            icon.lockedFill.gameObject.SetActive(hasCooldown);
            if (hasCooldown)
                icon.lockedFill.fillAmount = icon.currentCooldownFill;
        }
    }


}
