using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_SkillTree : MonoBehaviour
{
    [Header("Skill Buttons")]
    [SerializeField] private UI_SkillButton fireE_UI;
    [SerializeField] private UI_SkillButton fireR_UI;
    [SerializeField] private UI_SkillButton iceE_UI;
    [SerializeField] private UI_SkillButton iceR_UI;

    [Header("UI Components")]
    [SerializeField] private CanvasGroup canvasGroup;  // CanvasGroup của Skill_Tree
    [SerializeField] private Image dimBackground;      // Ảnh nền tối mờ (DimOverlay)
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private bool pauseGameWhenOpen = false; // Tùy chọn: tạm dừng game khi mở Skill Tree

    private PlayerSkill playerSkill;
    private PlayerController playerController;
    private PlayerAttack playerAttack;
    private UI_SkillBar skillBar;

    private bool isOpen = false;
    private Coroutine fadeUIRoutine;
    private Coroutine fadeDimRoutine;

    private void Awake()
    {
        // 🔹 Tìm Player và các script cần vô hiệu khi mở UI
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerSkill = playerObj.GetComponent<PlayerSkill>();
            playerController = playerObj.GetComponent<PlayerController>();
            playerAttack = playerObj.GetComponent<PlayerAttack>();
        }


        skillBar = FindFirstObjectByType<UI_SkillBar>();
        if (playerSkill == null)
        {
            Debug.LogError("❌ Không tìm thấy PlayerSkill trong Player!");
            return;
        }

        // 🔹 Gán sự kiện click cho từng nút skill
        if (fireE_UI != null)
            fireE_UI.GetComponent<Button>().onClick.AddListener(() => Unlock(PlayerSkill.SkillType.Fire_E));
        if (fireR_UI != null)
            fireR_UI.GetComponent<Button>().onClick.AddListener(() => Unlock(PlayerSkill.SkillType.Fire_R));
        if (iceE_UI != null)
            iceE_UI.GetComponent<Button>().onClick.AddListener(() => Unlock(PlayerSkill.SkillType.Ice_E));
        if (iceR_UI != null)
            iceR_UI.GetComponent<Button>().onClick.AddListener(() => Unlock(PlayerSkill.SkillType.Ice_R));

        // 🔹 Ẩn UI ban đầu
        SetCanvasVisible(false, true);
        SetDimVisible(false, true);
    }

    private void Start()
    {
        RefreshUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            ToggleSkillTree();

        if (Input.GetKeyDown(KeyCode.Escape) && isOpen)
            CloseSkillTree();
    }

    // 🔁 Toggle mở / đóng Skill Tree
    private void ToggleSkillTree()
    {
        isOpen = !isOpen;

        SetCanvasVisible(isOpen);
        SetDimVisible(isOpen);
        LockPlayerControls(isOpen);

        if (pauseGameWhenOpen)
            Time.timeScale = isOpen ? 0f : 1f;

        if (isOpen)
            RefreshUI();

        Debug.Log(isOpen ? "📜 Skill Tree mở!" : "❌ Skill Tree đóng!");
    }

    private void CloseSkillTree()
    {
        isOpen = false;
        SetCanvasVisible(false);
        SetDimVisible(false);
        LockPlayerControls(false);

        if (pauseGameWhenOpen)
            Time.timeScale = 1f;

        Debug.Log("❌ Skill Tree đã đóng (ESC).");
    }

    // 🔒 Khóa điều khiển player
    private void LockPlayerControls(bool locked)
    {
        if (playerController != null)
            playerController.enabled = !locked;

        if (playerAttack != null)
            playerAttack.inputLocked = locked;

        // Nếu bạn có thêm script khác (teleport, climb, detector...)
        // thì có thể thêm vào đây tương tự
    }

    // 🔧 Fade CanvasGroup UI
    private void SetCanvasVisible(bool visible, bool instant = false)
    {
        if (canvasGroup == null) return;

        if (fadeUIRoutine != null)
            StopCoroutine(fadeUIRoutine);

        fadeUIRoutine = StartCoroutine(FadeCanvasRoutine(canvasGroup, visible, instant));
    }

    private IEnumerator FadeCanvasRoutine(CanvasGroup cg, bool visible, bool instant)
    {
        float targetAlpha = visible ? 1f : 0f;
        float startAlpha = cg.alpha;
        float elapsed = 0f;

        cg.interactable = visible;
        cg.blocksRaycasts = visible;

        if (instant)
        {
            cg.alpha = targetAlpha;
            yield break;
        }

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        cg.alpha = targetAlpha;
    }

    // 🔧 Fade lớp nền mờ (overlay)
    private void SetDimVisible(bool visible, bool instant = false)
    {
        if (dimBackground == null) return;

        if (fadeDimRoutine != null)
            StopCoroutine(fadeDimRoutine);

        fadeDimRoutine = StartCoroutine(FadeImageRoutine(dimBackground, visible, instant));
    }

    private IEnumerator FadeImageRoutine(Image img, bool visible, bool instant)
    {
        Color color = img.color;
        float targetAlpha = visible ? 0.8f : 0f;
        float startAlpha = color.a;
        float elapsed = 0f;

        if (instant)
        {
            color.a = targetAlpha;
            img.color = color;
            yield break;
        }

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            img.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        img.color = color;
    }

    // 🔓 Unlock skill
    private void Unlock(PlayerSkill.SkillType type)
    {
        if (playerSkill == null)
        {
            Debug.LogError("❌ playerSkill NULL — không thể unlock!");
            return;
        }

        playerSkill.UnlockSkill(type);
        RefreshUI();

        if (skillBar != null)
        {
            skillBar.RefreshSkillBar();
            PlayerAttack.Element current = FindFirstObjectByType<PlayerAttack>().CurrentElement;
            skillBar.UpdateElementUI(current);
        }
            

        Debug.Log($"⭐ Unlocked: {type}");
    }

    // 🔁 Refresh UI skill icons
    private void RefreshUI()
    {
        if (playerSkill == null) return;

        if (fireE_UI != null)
            fireE_UI.SetUnlocked(playerSkill.IsSkillUnlocked(PlayerSkill.SkillType.Fire_E));
        if (fireR_UI != null)
            fireR_UI.SetUnlocked(playerSkill.IsSkillUnlocked(PlayerSkill.SkillType.Fire_R));
        if (iceE_UI != null)
            iceE_UI.SetUnlocked(playerSkill.IsSkillUnlocked(PlayerSkill.SkillType.Ice_E));
        if (iceR_UI != null)
            iceR_UI.SetUnlocked(playerSkill.IsSkillUnlocked(PlayerSkill.SkillType.Ice_R));
    }
}
