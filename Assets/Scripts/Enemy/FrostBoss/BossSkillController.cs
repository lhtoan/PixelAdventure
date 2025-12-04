// using UnityEngine;
// using System.Collections;

// public class BossSkillController : MonoBehaviour
// {
//     public Transform target;

//     [Header("Cooldown Settings")]
//     public float skillCooldown = 1.5f;

//     [Header("Attack Settings")]
//     [SerializeField] private float attackAnimDuration = 0.8f;

//     [Header("Skill Weights")]
//     [SerializeField] private int attackWeight = 10;
//     [SerializeField] private int skill1Weight = 10;
//     [SerializeField] private int skill2Weight = 80;

//     private Animator anim;
//     private BossAttack bossAttack;

//     [Header("Skill References")]
//     [SerializeField] private BossSkill1 skill1;
//     [SerializeField] private BossSkill2 skill2;

//     private bool isUsingSkill = false;

//     private void Awake()
//     {
//         anim = GetComponent<Animator>();
//         bossAttack = GetComponent<BossAttack>();
//     }

//     private void Start()
//     {
//         StartCoroutine(SkillLoop());
//     }

//     private IEnumerator SkillLoop()
//     {
//         while (true)
//         {
//             if (isUsingSkill)
//             {
//                 yield return null;
//                 continue;
//             }

//             if (target == null)
//             {
//                 yield return null;
//                 continue;
//             }

//             int skill = GetWeightedRandomSkill();
//             isUsingSkill = true;

//             switch (skill)
//             {
//                 case 0:
//                     Debug.Log("[BossSkill] Use Attack");

//                     if (!bossAttack.TryPerformAttack())
//                     {
//                         isUsingSkill = false;
//                         continue;
//                     }

//                     yield return new WaitForSeconds(attackAnimDuration);
//                     break;

//                 case 1:
//                     Debug.Log("[BossSkill] Use Skill 1");
//                     if (skill1 != null)
//                         yield return StartCoroutine(skill1.CastSkill1());
//                     break;

//                 case 2:
//                     Debug.Log("[BossSkill] Use Skill 2");
//                     if (skill2 != null)
//                         yield return StartCoroutine(skill2.CastSkill2()); // ⭐ GIỐNG SKILL 1
//                     break;
//             }

//             yield return new WaitForSeconds(skillCooldown);
//             isUsingSkill = false;
//         }
//     }

//     private int GetWeightedRandomSkill()
//     {
//         int total = attackWeight + skill1Weight + skill2Weight;
//         int rand = Random.Range(0, total);

//         if (rand < attackWeight)
//             return 0;

//         rand -= attackWeight;

//         if (rand < skill1Weight)
//             return 1;

//         return 2;
//     }

//     public bool IsUsingSkill => isUsingSkill;
// }
using UnityEngine;
using System.Collections;

public class BossSkillController : MonoBehaviour
{
    public Transform target;

    [Header("Cooldown Settings")]
    public float skillCooldown = 1.5f;     // cooldown giữa các skill
    [SerializeField] private float postSkillDelay = 2f; // ⭐ delay sau khi tung skill xong

    [Header("Attack Settings")]
    [SerializeField] private float attackAnimDuration = 0.8f;

    [Header("Skill Weights")]
    [SerializeField] private int attackWeight = 10;
    [SerializeField] private int skill1Weight = 10;
    [SerializeField] private int skill2Weight = 80;

    private Animator anim;
    private BossAttack bossAttack;

    [Header("Skill References")]
    [SerializeField] private BossSkill1 skill1;
    [SerializeField] private BossSkill2 skill2;

    private bool isUsingSkill = false;
    private int lastSkill = -1;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        bossAttack = GetComponent<BossAttack>();
    }

    private void Start()
    {
        StartCoroutine(SkillLoop());
    }

    private IEnumerator SkillLoop()
    {
        while (true)
        {
            if (isUsingSkill)
            {
                yield return null;
                continue;
            }

            if (target == null)
            {
                yield return null;
                continue;
            }

            int skill = GetWeightedRandomSkill();
            lastSkill = skill;

            isUsingSkill = true;

            switch (skill)
            {
                case 0:
                    Debug.Log("[BossSkill] Use Attack");

                    if (!bossAttack.TryPerformAttack())
                    {
                        isUsingSkill = false;
                        continue;
                    }

                    yield return new WaitForSeconds(attackAnimDuration);
                    break;

                case 1:
                    Debug.Log("[BossSkill] Use Skill 1");
                    if (skill1 != null)
                        yield return StartCoroutine(skill1.CastSkill1());
                    break;

                case 2:
                    Debug.Log("[BossSkill] Use Skill 2");
                    if (skill2 != null)
                        yield return StartCoroutine(skill2.CastSkill2());
                    break;
            }

            // ⭐ Cooldown bình thường
            yield return new WaitForSeconds(skillCooldown);

            // ⭐ Thêm delay sau khi tung skill
            yield return new WaitForSeconds(postSkillDelay);

            isUsingSkill = false;
        }
    }

    private int GetWeightedRandomSkill()
    {
        while (true)
        {
            int total = attackWeight + skill1Weight + skill2Weight;
            int rand = Random.Range(0, total);

            int skill = -1;

            if (rand < attackWeight)
                skill = 0;
            else if (rand < attackWeight + skill1Weight)
                skill = 1;
            else
                skill = 2;

            if (skill != lastSkill)
                return skill;
        }
    }

    public bool IsUsingSkill => isUsingSkill;
}
