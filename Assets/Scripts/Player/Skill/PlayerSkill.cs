using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    public enum SkillType
    {
        Fire_E,
        Fire_R,
        Ice_E,
        Ice_R
    }

    private List<SkillType> unlockedSkillTypeList;

    private void Awake()
    {
        unlockedSkillTypeList = new List<SkillType>();
    }

    public void UnlockSkill(SkillType skillType)
    {
        if (!unlockedSkillTypeList.Contains(skillType))
        {
            unlockedSkillTypeList.Add(skillType);
            Debug.Log("Unlock skill: " + skillType);
        }
    }

    public bool IsSkillUnlocked(SkillType skillType)
    {
        return unlockedSkillTypeList.Contains(skillType);
    }
}
