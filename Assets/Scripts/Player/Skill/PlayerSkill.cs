using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    public enum SkillType
    {
        Fire_E,
        Fire_R,
        Ice_E,
        Ice_R,
        Health_Up,
        Health_Up_2,
        Stamina_Up,
        Stamina_Up_2,
        Fire_Cooldown
        
    }

    [Header("Debug Options")]
    [SerializeField] private bool debugUnlockAll = false;

    private List<SkillType> unlockedSkillTypeList;

    private Dictionary<SkillType, List<SkillType>> prerequisite = new()
    {
        { SkillType.Fire_E, new List<SkillType>{ } },
        { SkillType.Health_Up, new List<SkillType>{ SkillType.Fire_E } },
        { SkillType.Stamina_Up, new List<SkillType>() },
        { SkillType.Health_Up_2, new List<SkillType>()},
        { SkillType.Fire_Cooldown, new List<SkillType>{ SkillType.Stamina_Up, SkillType.Fire_E } },
        { SkillType.Stamina_Up_2, new List<SkillType>{ SkillType.Health_Up_2 } },
        { SkillType.Fire_R, new List<SkillType>{ SkillType.Fire_Cooldown, SkillType.Stamina_Up_2 } }

    };


    private void Awake()
    {
        unlockedSkillTypeList = new List<SkillType>();
    }

    private void Start()
    {
        if (debugUnlockAll)
        {
            Debug.Log("⚡ DEBUG MODE: Tự động mở tất cả skill!");

            foreach (SkillType type in System.Enum.GetValues(typeof(SkillType)))
            {
                unlockedSkillTypeList.Add(type);
            }
        }
    }

    // ⭐ KIỂM TRA điều kiện unlock
    public bool CanUnlock(SkillType skillType)
    {
        if (prerequisite.ContainsKey(skillType))
        {
            foreach (SkillType req in prerequisite[skillType])
            {
                if (!unlockedSkillTypeList.Contains(req))
                {
                    Debug.Log($"❌ Không thể mở {skillType}! Cần mở trước: {req}");
                    return false;
                }
            }
        }

        return true;
    }


    public bool UnlockSkill(SkillType skillType)
    {
        if (unlockedSkillTypeList.Contains(skillType))
            return false;

        if (!CanUnlock(skillType))
            return false;

        unlockedSkillTypeList.Add(skillType);
        Debug.Log($"⭐ Unlock: {skillType}");

        // ⭐ Thưởng hiệu ứng khi mở từng skill
        var hp = GetComponent<Health>();
        var st = GetComponent<PlayerStamina>();

        switch (skillType)
        {
            case SkillType.Health_Up:
                hp?.IncreaseMaxHealth(1);
                break;

            case SkillType.Stamina_Up:
                st?.IncreaseMaxStamina(10);
                break;

            case SkillType.Health_Up_2:
                hp?.IncreaseMaxHealth(1);
                break;
            case SkillType.Stamina_Up_2:
                st?.IncreaseMaxStamina(50);
                st?.IncreaseRegenRate(7);
                break;
            case SkillType.Fire_Cooldown:

                float reducePercent = 0.10f; // giảm 10%

                Skill_E_Fire fireE = GetComponentInChildren<Skill_E_Fire>();
                Skill_R_Fire fireR = GetComponentInChildren<Skill_R_Fire>();

                if (fireE != null)
                    fireE.ApplyCooldownUpgrade(reducePercent);

                if (fireR != null)
                    fireR.ApplyCooldownUpgrade(reducePercent);

                PlayerAttack atk = GetComponent<PlayerAttack>();
                if (atk != null)
                {
                    atk.burnDamageBonus += 0.30f;      // +20% DOT damage
                }
                break;

        }

        return true;
    }


    public bool IsSkillUnlocked(SkillType skillType)
    {
        return unlockedSkillTypeList.Contains(skillType);
    }

    public Dictionary<SkillType, List<SkillType>> GetPrerequisite()
    {
        return prerequisite;
    }

}
