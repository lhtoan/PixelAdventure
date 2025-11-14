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
        RefreshSkillBar();
        PlayerAttack.Element current = FindFirstObjectByType<PlayerAttack>().CurrentElement;
        UpdateElementUI(current);
    }

    public void RefreshSkillBar()
    {
        CheckUnlockAnimation(fireE_Icon, PlayerSkill.SkillType.Fire_E);
        CheckUnlockAnimation(fireR_Icon, PlayerSkill.SkillType.Fire_R);
        CheckUnlockAnimation(iceE_Icon, PlayerSkill.SkillType.Ice_E);
        CheckUnlockAnimation(iceR_Icon, PlayerSkill.SkillType.Ice_R);
    }

    // 🟢 Check + chạy hiệu ứng unlock
    private void CheckUnlockAnimation(GameObject icon, PlayerSkill.SkillType type)
    {
        bool unlocked = playerSkill.IsSkillUnlocked(type);

        // Skill chưa unlock → ẩn icon
        if (!unlocked)
        {
            icon.SetActive(false);
            return;
        }

        // Nếu icon vừa được bật lần đầu → chạy animation
        if (!icon.activeSelf)
        {
            icon.SetActive(true);
            StartCoroutine(UnlockAnimation(icon.transform));
        }
    }

    // 🔥 Animation pop + fade
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

        // Fade + scale tăng
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime * 1.5f;
            target.localScale = Vector3.Lerp(small, big, t / time);
            cg.alpha = t / time;
            yield return null;
        }

        // Thu nhỏ về scale chuẩn
        t = 0f;
        while (t < time)
        {
            t += Time.deltaTime * 2f;
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

    private void UpdateOneIcon(GameObject iconObj, bool isUnlockedForCurrentElement)
    {
        UI_SkillBarIcon icon = iconObj.GetComponent<UI_SkillBarIcon>();
        if (icon == null) return;

        // icon màu bật nếu đang ở hệ tương ứng
        icon.unlockedIcon.SetActive(isUnlockedForCurrentElement);

        // lockedFill bật nếu KHÔNG phù hợp hệ
        icon.lockedFill.gameObject.SetActive(!isUnlockedForCurrentElement);

        // đảm bảo fillAmount = 0 nếu không cooldown
        if (!isUnlockedForCurrentElement)
            icon.lockedFill.fillAmount = 1f; // full grayscale
    }


}
