using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UI_SkillTree : MonoBehaviour
{
    // ==============================
    //      INSPECTOR FIELDS
    // ==============================
    [Header("Skill Buttons")]
    [SerializeField] private UI_SkillButton fireE_UI;
    [SerializeField] private UI_SkillButton fireR_UI;
    [SerializeField] private UI_SkillButton iceE_UI;
    [SerializeField] private UI_SkillButton iceR_UI;
    [SerializeField] private UI_SkillButton healthUp_UI;
    [SerializeField] private UI_SkillButton staminaUp_UI;
    [SerializeField] private UI_SkillButton healthUp2_UI;
    [SerializeField] private UI_SkillButton staminaUp2_UI;
    [SerializeField] private UI_SkillButton fireCooldown_UI;
    [SerializeField] private UI_SkillButton iceStack_UI;
    [SerializeField] private UI_SkillButton treasure_UI;

    [Header("Skill Costs")]
    public int fireE_SkillCost = 1;
    public int fireE_CoinCost = 100;
    public int fireR_SkillCost = 2;
    public int fireR_CoinCost = 150;
    public int iceE_SkillCost = 1;
    public int iceE_CoinCost = 80;
    public int iceR_SkillCost = 2;
    public int iceR_CoinCost = 120;

    [Header("Extra Skill Costs")]
    public int healthUp_SkillCost = 0;
    public int healthUp_CoinCost = 120;
    public int staminaUp_SkillCost = 0;
    public int staminaUp_CoinCost = 140;
    public int healthUp2_SkillCost = 0;
    public int healthUp2_CoinCost = 180;
    public int fireCooldown_SkillCost = 0;
    public int fireCooldown_CoinCost = 180;
    public int staminaUp2_SkillCost = 0;
    public int staminaUp2_CoinCost = 180;
    public int iceStack_SkillCost = 0;
    public int iceStack_CoinCost = 180;
    public int treasure_SkillCost = 0;
    public int treasure_CoinCost = 180;

    [Header("UI Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image dimBackground;
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private bool pauseGameWhenOpen = false;

    // ==============================
    //      INTERNAL FIELDS
    // ==============================
    public static UI_SkillTree Instance;

    private PlayerSkill playerSkill;
    private PlayerController playerController;
    private PlayerAttack playerAttack;
    private UI_SkillBar skillBar;

    private Coroutine fadeUIRoutine;
    private Coroutine fadeDimRoutine;
    private bool isOpen = false;

    public Dictionary<PlayerSkill.SkillType, List<PlayerSkill.SkillType>> prerequisite;
    public Dictionary<PlayerSkill.SkillType, UI_SkillButton> buttonLookup =
        new Dictionary<PlayerSkill.SkillType, UI_SkillButton>();


    // ==============================
    //          AWAKE
    // ==============================
    private void Awake()
    {
        Instance = this;

        SetupButtonLookup();
        AssignSkillTypes();
        FindPlayerReferences();

        prerequisite = playerSkill.GetPrerequisite();

        AssignButtonClickHandlers();

        SetCanvasVisible(false, true);
        SetDimVisible(false, true);
    }

    // ==============================
    //          SETUP HELPERS
    // ==============================

    private void SetupButtonLookup()
    {
        buttonLookup.Clear();

        buttonLookup[PlayerSkill.SkillType.Fire_E] = fireE_UI;
        buttonLookup[PlayerSkill.SkillType.Fire_R] = fireR_UI;
        buttonLookup[PlayerSkill.SkillType.Health_Up] = healthUp_UI;
        buttonLookup[PlayerSkill.SkillType.Stamina_Up] = staminaUp_UI;
        buttonLookup[PlayerSkill.SkillType.Health_Up_2] = healthUp2_UI;
        buttonLookup[PlayerSkill.SkillType.Stamina_Up_2] = staminaUp2_UI;
        buttonLookup[PlayerSkill.SkillType.Fire_Cooldown] = fireCooldown_UI;


        buttonLookup[PlayerSkill.SkillType.Ice_E] = iceE_UI;
        buttonLookup[PlayerSkill.SkillType.Ice_R] = iceR_UI;
        buttonLookup[PlayerSkill.SkillType.IceStack] = iceStack_UI;
        buttonLookup[PlayerSkill.SkillType.Treasure] = treasure_UI;

    }

    private void AssignSkillTypes()
    {
        fireE_UI.skillType = PlayerSkill.SkillType.Fire_E;
        fireR_UI.skillType = PlayerSkill.SkillType.Fire_R;
        healthUp_UI.skillType = PlayerSkill.SkillType.Health_Up;
        staminaUp_UI.skillType = PlayerSkill.SkillType.Stamina_Up;
        healthUp2_UI.skillType = PlayerSkill.SkillType.Health_Up_2;
        staminaUp2_UI.skillType = PlayerSkill.SkillType.Stamina_Up_2;
        fireCooldown_UI.skillType = PlayerSkill.SkillType.Fire_Cooldown;


        iceE_UI.skillType = PlayerSkill.SkillType.Ice_E;
        iceR_UI.skillType = PlayerSkill.SkillType.Ice_R;
        iceStack_UI.skillType = PlayerSkill.SkillType.IceStack;
        treasure_UI.skillType = PlayerSkill.SkillType.Treasure;

    }

    private void FindPlayerReferences()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");

        if (obj != null)
        {
            playerSkill = obj.GetComponent<PlayerSkill>();
            playerController = obj.GetComponent<PlayerController>();
            playerAttack = obj.GetComponent<PlayerAttack>();
        }

        skillBar = FindFirstObjectByType<UI_SkillBar>();
    }

    private void AssignButtonClickHandlers()
    {
        fireE_UI?.GetComponent<Button>().onClick.AddListener(() => Unlock(PlayerSkill.SkillType.Fire_E));
        fireR_UI?.GetComponent<Button>().onClick.AddListener(() => Unlock(PlayerSkill.SkillType.Fire_R));
        healthUp_UI?.GetComponent<Button>().onClick.AddListener(() => Unlock(PlayerSkill.SkillType.Health_Up));
        staminaUp_UI?.GetComponent<Button>().onClick.AddListener(() => Unlock(PlayerSkill.SkillType.Stamina_Up));
        healthUp2_UI?.GetComponent<Button>().onClick.AddListener(() => Unlock(PlayerSkill.SkillType.Health_Up_2));
        fireCooldown_UI?.GetComponent<Button>().onClick.AddListener(() => Unlock(PlayerSkill.SkillType.Fire_Cooldown));
        staminaUp2_UI?.GetComponent<Button>().onClick.AddListener(() => Unlock(PlayerSkill.SkillType.Stamina_Up_2));

        iceE_UI?.GetComponent<Button>().onClick.AddListener(() => Unlock(PlayerSkill.SkillType.Ice_E));
        iceR_UI?.GetComponent<Button>().onClick.AddListener(() => Unlock(PlayerSkill.SkillType.Ice_R));
        iceStack_UI?.GetComponent<Button>().onClick.AddListener(() => Unlock(PlayerSkill.SkillType.IceStack));
        treasure_UI?.GetComponent<Button>().onClick.AddListener(() => Unlock(PlayerSkill.SkillType.Treasure));

    }


    // ==============================
    //          START + UPDATE
    // ==============================
    private void Start() => RefreshUI();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            ToggleSkillTree();

        if (Input.GetKeyDown(KeyCode.Escape) && isOpen)
            CloseSkillTree();
    }

    // ==============================
    //          OPEN / CLOSE UI
    // ==============================
    private void ToggleSkillTree()
    {
        isOpen = !isOpen;
        SetCanvasVisible(isOpen);
        SetDimVisible(isOpen);
        LockPlayerControls(isOpen);

        if (pauseGameWhenOpen)
            Time.timeScale = isOpen ? 0 : 1;

        if (isOpen)
            RefreshUI();
    }

    private void CloseSkillTree()
    {
        isOpen = false;
        SetCanvasVisible(false);
        SetDimVisible(false);
        LockPlayerControls(false);

        if (pauseGameWhenOpen)
            Time.timeScale = 1;
    }

    // ==============================
    //       LOCK PLAYER INPUT
    // ==============================
    private void LockPlayerControls(bool locked)
    {
        if (playerController)
            playerController.enabled = !locked;

        if (playerAttack)
            playerAttack.inputLocked = locked;
    }

    // ==============================
    //         UI FADING
    // ==============================
    private void SetCanvasVisible(bool visible, bool instant = false)
    {
        if (!canvasGroup) return;

        if (fadeUIRoutine != null)
            StopCoroutine(fadeUIRoutine);

        fadeUIRoutine = StartCoroutine(FadeCanvasRoutine(canvasGroup, visible, instant));
    }

    private IEnumerator FadeCanvasRoutine(CanvasGroup cg, bool visible, bool instant)
    {
        float target = visible ? 1f : 0f;

        if (instant)
        {
            cg.alpha = target;
            cg.blocksRaycasts = visible;
            cg.interactable = visible;
            yield break;
        }

        float start = cg.alpha;
        float t = 0;

        cg.blocksRaycasts = visible;
        cg.interactable = visible;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }

        cg.alpha = target;
    }

    private void SetDimVisible(bool visible, bool instant = false)
    {
        if (!dimBackground) return;

        if (fadeDimRoutine != null)
            StopCoroutine(fadeDimRoutine);

        fadeDimRoutine = StartCoroutine(FadeImageRoutine(dimBackground, visible, instant));
    }

    private IEnumerator FadeImageRoutine(Image img, bool visible, bool instant)
    {
        float target = visible ? 0.8f : 0f;

        if (instant)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, target);
            yield break;
        }

        float start = img.color.a;
        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(start, target, t / fadeDuration);
            img.color = new Color(img.color.r, img.color.g, img.color.b, a);
            yield return null;
        }
    }

    // ==============================
    //      UNLOCK + REFRESH UI
    // ==============================
    private void Unlock(PlayerSkill.SkillType type)
    {
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (!gm || !playerSkill)
            return;

        // Cost lookup
        int sp = 0, coin = 0;
        GetCosts(type, ref sp, ref coin);

        if (gm.SkillPoints < sp || gm.Score < coin)
            return;

        if (!playerSkill.UnlockSkill(type))
            return;

        gm.AddSkillPoint(-sp);
        gm.AddScore(-coin);

        RefreshUI();

        skillBar?.RefreshSkillBar();
        skillBar?.UpdateElementUI(FindFirstObjectByType<PlayerAttack>().CurrentElement);
    }

    private void GetCosts(PlayerSkill.SkillType type, ref int sp, ref int coin)
    {
        switch (type)
        {
            case PlayerSkill.SkillType.Fire_E: sp = fireE_SkillCost; coin = fireE_CoinCost; break;
            case PlayerSkill.SkillType.Fire_R: sp = fireR_SkillCost; coin = fireR_CoinCost; break;
            case PlayerSkill.SkillType.Health_Up: sp = healthUp_SkillCost; coin = healthUp_CoinCost; break;
            case PlayerSkill.SkillType.Stamina_Up: sp = staminaUp_SkillCost; coin = staminaUp_CoinCost; break;
            case PlayerSkill.SkillType.Health_Up_2: sp = healthUp2_SkillCost; coin = healthUp2_CoinCost; break;
            case PlayerSkill.SkillType.Fire_Cooldown: sp = fireCooldown_SkillCost; coin = fireCooldown_CoinCost; break;
            case PlayerSkill.SkillType.Stamina_Up_2: sp = staminaUp2_SkillCost; coin = staminaUp2_CoinCost; break;

            case PlayerSkill.SkillType.Ice_E: sp = iceE_SkillCost; coin = iceE_CoinCost; break;
            case PlayerSkill.SkillType.Ice_R: sp = iceR_SkillCost; coin = iceR_CoinCost; break;
            case PlayerSkill.SkillType.IceStack: sp = iceStack_SkillCost; coin = iceStack_CoinCost; break;
            case PlayerSkill.SkillType.Treasure:sp = treasure_SkillCost; coin = treasure_CoinCost; break;

        }
    }

    private void RefreshUI()
    {
        fireE_UI.SetUnlocked(playerSkill.IsSkillUnlocked(PlayerSkill.SkillType.Fire_E));
        fireR_UI.SetUnlocked(playerSkill.IsSkillUnlocked(PlayerSkill.SkillType.Fire_R));
        healthUp_UI.SetUnlocked(playerSkill.IsSkillUnlocked(PlayerSkill.SkillType.Health_Up));
        staminaUp_UI.SetUnlocked(playerSkill.IsSkillUnlocked(PlayerSkill.SkillType.Stamina_Up));
        healthUp2_UI.SetUnlocked(playerSkill.IsSkillUnlocked(PlayerSkill.SkillType.Health_Up_2));
        fireCooldown_UI.SetUnlocked(playerSkill.IsSkillUnlocked(PlayerSkill.SkillType.Fire_Cooldown));
        staminaUp2_UI.SetUnlocked(playerSkill.IsSkillUnlocked(PlayerSkill.SkillType.Stamina_Up_2));


        iceE_UI.SetUnlocked(playerSkill.IsSkillUnlocked(PlayerSkill.SkillType.Ice_E));
        iceR_UI.SetUnlocked(playerSkill.IsSkillUnlocked(PlayerSkill.SkillType.Ice_R));
        iceStack_UI.SetUnlocked(playerSkill.IsSkillUnlocked(PlayerSkill.SkillType.IceStack));
        treasure_UI.SetUnlocked(playerSkill.IsSkillUnlocked(PlayerSkill.SkillType.Treasure));

    }
}
