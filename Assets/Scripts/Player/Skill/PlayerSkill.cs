using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    public enum SkillType
    {
        //Fire
        Fire_E,
        Fire_R,
        Health_Up,
        Health_Up_2,
        Stamina_Up,
        Stamina_Up_2,
        Fire_Cooldown,

        //Ice
        Ice_E,
        Ice_R,
        Treasure,
        IceStack


    }

    [Header("Debug Options")]
    [SerializeField] private bool debugUnlockAll = false;

    private List<SkillType> unlockedSkillTypeList;

    private Dictionary<SkillType, List<SkillType>> prerequisite = new()
    {
        { SkillType.Fire_E, new List<SkillType>{ } },
        { SkillType.Health_Up, new List<SkillType>{ SkillType.Fire_E } },
        { SkillType.Stamina_Up, new List<SkillType>{ SkillType.Fire_E } },
        { SkillType.Fire_Cooldown, new List<SkillType>{ SkillType.Health_Up, SkillType.Fire_E } },
        { SkillType.Health_Up_2, new List<SkillType>{ SkillType.Stamina_Up } },
        { SkillType.Stamina_Up_2, new List<SkillType>{ SkillType.Fire_Cooldown } },
        { SkillType.Fire_R, new List<SkillType>{ SkillType.Fire_Cooldown, SkillType.Stamina_Up_2, SkillType.Health_Up_2 } },


        { SkillType.Ice_E, new List<SkillType>{ } },
        { SkillType.Treasure, new List<SkillType>{ } },
        { SkillType.IceStack, new List<SkillType>{ SkillType.Treasure, SkillType.Ice_E } },
        { SkillType.Ice_R, new List<SkillType>{ SkillType.IceStack} },


    };

    private Dictionary<SkillType, string> skillDescriptions = new()
    {
        { SkillType.Fire_E,        "Shoots multiple fast-moving fireballs in all directions, each dealing damage and burning enemies on hit. [Cooldown: 10 seconds]" },
        { SkillType.Fire_R,        "Summons a homing fire orb that burns and damages nearby enemies over time. [Cooldown: 15s]" },

        { SkillType.Health_Up,     "Increases Max HP by 1." },
        { SkillType.Health_Up_2,   "Increases Max HP by 1." },

        { SkillType.Stamina_Up,    "Increases Max Stamina by 10." },
        { SkillType.Stamina_Up_2,  "Increases Max Stamina by 40 and boosts Stamina regeneration by +10 per second." },

        { SkillType.Fire_Cooldown, "Reduces cooldown of all Fire skills by 30% and increases Burn damage by 30%." },

        { SkillType.Ice_E,         "Creates a protective ice shield for a short duration, dealing periodic damage and applying Freeze effects to enemies on contact. [Cooldown: 8 seconds]." },
        { SkillType.Ice_R,         "Summons Ice Pillars forward, dealing multiple hits and freezing enemies on contact. [Cooldown: 10 second]" },

        { SkillType.Treasure,      "Grants a 50% chance to double Coins and enemy drop rewards." },
        { SkillType.IceStack,      "Ice attacks apply stacks to enemies. Every 3 stacks triggers an Ice Burst dealing accumulated Ice damage and freezes the target if not already frozen." },
    };


    public string GetSkillDescription(SkillType skill)
    {
        if (skillDescriptions.TryGetValue(skill, out string desc))
            return desc;

        return "Chưa có mô tả.";
    }



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
                st?.IncreaseRegenRate(10);
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

    // ===============================================================
    //         TREASURE BONUS — 50% cơ hội nhân đôi phần thưởng
    // ===============================================================

    public bool HasTreasureBonus()
    {
        return unlockedSkillTypeList.Contains(SkillType.Treasure);
    }

    public int ApplyTreasureBonus(int baseValue)
    {
        if (!HasTreasureBonus())
            return baseValue;

        // 50% tỉ lệ x2
        if (Random.value <= 0.5f)
        {
            Debug.Log("🎁 TREASURE BONUS x2!");
            return baseValue * 2;
        }

        return baseValue;
    }

    public float ApplyTreasureBonus(float baseValue)
    {
        if (!HasTreasureBonus())
            return baseValue;

        if (Random.value <= 0.5f)
        {
            Debug.Log("🎁 TREASURE BONUS x2!");
            return baseValue * 2f;
        }

        return baseValue;
    }

    public List<string> GetUnlockedSkillsList()
    {
        List<string> list = new List<string>();
        foreach (SkillType s in unlockedSkillTypeList)
            list.Add(s.ToString());
        return list;
    }

    public void LoadUnlockedSkills(List<string> skills)
    {
        unlockedSkillTypeList.Clear();

        foreach (string s in skills)
        {
            if (System.Enum.TryParse(s, out SkillType skill))
            {
                unlockedSkillTypeList.Add(skill);

                // áp dụng hiệu ứng ngay lập tức
                ApplySkillEffects(skill);
            }
        }
    }


    private void ApplySkillEffects(SkillType skillType)
    {
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
                st?.IncreaseMaxStamina(40);
                st?.IncreaseRegenRate(10);
                break;

            case SkillType.Fire_Cooldown:
                float reducePercent = 0.30f;

                Skill_E_Fire fireE = GetComponentInChildren<Skill_E_Fire>();
                Skill_R_Fire fireR = GetComponentInChildren<Skill_R_Fire>();

                fireE?.ApplyCooldownUpgrade(reducePercent);
                fireR?.ApplyCooldownUpgrade(reducePercent);

                PlayerAttack atk = GetComponent<PlayerAttack>();
                if (atk != null)
                    atk.burnDamageBonus += 0.30f;
                break;
        }
    }




}
