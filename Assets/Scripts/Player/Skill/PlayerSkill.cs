// using System.Collections.Generic;
// using UnityEngine;

// public class PlayerSkill : MonoBehaviour
// {
//     public enum SkillType
//     {
//         Fire_E,
//         Fire_R,
//         Ice_E,
//         Ice_R
//     }

//     private List<SkillType> unlockedSkillTypeList;

//     private void Awake()
//     {
//         unlockedSkillTypeList = new List<SkillType>();
//     }

//     public void UnlockSkill(SkillType skillType)
//     {
//         if (!unlockedSkillTypeList.Contains(skillType))
//         {
//             unlockedSkillTypeList.Add(skillType);
//             Debug.Log("Unlock skill: " + skillType);
//         }
//     }

//     public bool IsSkillUnlocked(SkillType skillType)
//     {
//         return unlockedSkillTypeList.Contains(skillType);
//     }
// }
// using System.Collections.Generic;
// using UnityEngine;

// public class PlayerSkill : MonoBehaviour
// {
//     public enum SkillType
//     {
//         Fire_E,
//         Fire_R,
//         Ice_E,
//         Ice_R
//     }

//     [Header("Debug Options")]
//     [SerializeField] private bool debugUnlockAll = false;

//     private List<SkillType> unlockedSkillTypeList;

//     private void Awake()
//     {
//         unlockedSkillTypeList = new List<SkillType>();
//     }

//     private void Start()
//     {
//         if (debugUnlockAll)
//         {
//             Debug.Log("⚡ DEBUG MODE: Tự động mở tất cả skill!");

//             foreach (SkillType type in System.Enum.GetValues(typeof(SkillType)))
//             {
//                 if (!unlockedSkillTypeList.Contains(type))
//                     unlockedSkillTypeList.Add(type);
//             }
//         }
//     }

//     public void UnlockSkill(SkillType skillType)
//     {
//         if (!unlockedSkillTypeList.Contains(skillType))
//         {
//             unlockedSkillTypeList.Add(skillType);
//             Debug.Log("Unlock skill: " + skillType);
//         }
//     }

//     public bool IsSkillUnlocked(SkillType skillType)
//     {
//         return unlockedSkillTypeList.Contains(skillType);
//     }
// }
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
        Stamina_Up,
        Health_Up_2
    }

    [Header("Debug Options")]
    [SerializeField] private bool debugUnlockAll = false;

    private List<SkillType> unlockedSkillTypeList;

    // 🔥 Các yêu cầu để mở từng skill
    // private Dictionary<SkillType, SkillType> prerequisite = new()
    // {
    //     { SkillType.Health_Up, SkillType.Fire_E },        // Health_Up sau Fire_E
    //     { SkillType.Stamina_Up, SkillType.Health_Up },    // Stamina sau Health
    //     { SkillType.Fire_R, SkillType.Stamina_Up },        // Fire_R sau Stamina
    //     { SkillType.Health_Up_2, SkillType.Fire_R }        // Health_Up_2 sau Fire_R


    // };

    private Dictionary<SkillType, List<SkillType>> prerequisite = new()
    {
        { SkillType.Health_Up,     new List<SkillType>{ SkillType.Fire_E } },
        { SkillType.Stamina_Up,    new List<SkillType>{ SkillType.Fire_E } },
        { SkillType.Fire_R,        new List<SkillType>{ SkillType.Health_Up, SkillType.Stamina_Up } },
        { SkillType.Health_Up_2,   new List<SkillType>{ SkillType.Fire_R } }
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


    // ⭐ UNLOCK SKILL
    // public bool UnlockSkill(SkillType skillType)
    // {
    //     // 1️⃣ Kiểm tra đã mở chưa
    //     if (unlockedSkillTypeList.Contains(skillType))
    //     {
    //         Debug.Log($"⚠ Skill {skillType} đã mở trước đó.");
    //         return false;
    //     }

    //     // 2️⃣ Kiểm tra prerequisite
    //     if (!CanUnlock(skillType))
    //         return false;

    //     // 3️⃣ Thêm skill vào danh sách đã mở
    //     unlockedSkillTypeList.Add(skillType);
    //     Debug.Log($"⭐ Mở khóa skill: {skillType}");

    //     return true;
    // }

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
        }

        return true;
    }


    public bool IsSkillUnlocked(SkillType skillType)
    {
        return unlockedSkillTypeList.Contains(skillType);
    }
}
