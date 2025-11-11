// using UnityEngine;
// using UnityEngine.UI;

// public class UI_SkillTree : MonoBehaviour
// {
//     [Header("Skill Buttons")]
//     [SerializeField] private Button fireEButton;
//     [SerializeField] private Button fireRButton;
//     [SerializeField] private Button iceEButton;
//     [SerializeField] private Button iceRButton;

//     private PlayerSkill playerSkill;

//     private void Awake()
//     {
//         // ⭐ Tìm PlayerSkill từ Player TAG
//         GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
//         if (playerObj != null)
//         {
//             playerSkill = playerObj.GetComponent<PlayerSkill>();
//         }
//         else
//         {
//             Debug.LogError("❌ Không tìm thấy Player có Tag 'Player'!");
//             return;
//         }

//         // ⭐ Gán onclick cho từng nút nếu tồn tại
//         if (fireEButton != null)
//             fireEButton.onClick.AddListener(() => Unlock(PlayerSkill.SkillType.Fire_E));

//         if (fireRButton != null)
//             fireRButton.onClick.AddListener(() => Unlock(PlayerSkill.SkillType.Fire_R));

//         if (iceEButton != null)
//             iceEButton.onClick.AddListener(() => Unlock(PlayerSkill.SkillType.Ice_E));

//         if (iceRButton != null)
//             iceRButton.onClick.AddListener(() => Unlock(PlayerSkill.SkillType.Ice_R));
//     }

//     private void Unlock(PlayerSkill.SkillType type)
//     {
//         if (playerSkill == null)
//         {
//             Debug.LogError("❌ PlayerSkill chưa được gán!");
//             return;
//         }

//         playerSkill.UnlockSkill(type);
//         Debug.Log("⭐ Unlocked: " + type);
//     }
// }
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillTree : MonoBehaviour
{
    [Header("Skill Buttons")]
    [SerializeField] private UI_SkillButton fireE_UI;
    [SerializeField] private UI_SkillButton fireR_UI;
    [SerializeField] private UI_SkillButton iceE_UI;
    [SerializeField] private UI_SkillButton iceR_UI;

    private PlayerSkill playerSkill;

    private void Awake()
    {
        // ⭐ Tìm PlayerSkill từ player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerSkill = playerObj.GetComponent<PlayerSkill>();
        }

        if (playerSkill == null)
        {
            Debug.LogError("❌ Không tìm thấy PlayerSkill trong Player!");
            return;
        }

        // ⭐ Gán sự kiện click cho từng nút
        if (fireE_UI != null)
            fireE_UI.GetComponent<Button>().onClick.AddListener(() => Unlock(PlayerSkill.SkillType.Fire_E));

        if (fireR_UI != null)
            fireR_UI.GetComponent<Button>().onClick.AddListener(() => Unlock(PlayerSkill.SkillType.Fire_R));

        if (iceE_UI != null)
            iceE_UI.GetComponent<Button>().onClick.AddListener(() => Unlock(PlayerSkill.SkillType.Ice_E));

        if (iceR_UI != null)
            iceR_UI.GetComponent<Button>().onClick.AddListener(() => Unlock(PlayerSkill.SkillType.Ice_R));
    }

    private void Start()
    {
        // ⭐ Refresh UI sau khi tất cả Awake() đã chạy
        RefreshUI();
    }

    private void Unlock(PlayerSkill.SkillType type)
    {
        if (playerSkill == null)
        {
            Debug.LogError("❌ playerSkill NULL — không thể unlock!");
            return;
        }

        playerSkill.UnlockSkill(type);

        RefreshUI();

        Debug.Log($"⭐ Unlocked: {type}");
    }

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
