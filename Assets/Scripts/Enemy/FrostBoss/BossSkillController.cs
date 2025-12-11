// using UnityEngine;
// using System.Collections;

// public class BossSkillController : MonoBehaviour
// {
//     public Transform target;

//     [Header("Cooldown Settings")]
//     public float skillCooldown = 1.5f;
//     [SerializeField] private float postSkillDelay = 2f;

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

//     private FrostBossFollow bossFollow;   // ⭐ Thêm follow check
//     private bool isUsingSkill = false;
//     private int lastSkill = -1;

//     private void Awake()
//     {
//         anim = GetComponent<Animator>();
//         bossAttack = GetComponent<BossAttack>();
//         bossFollow = GetComponent<FrostBossFollow>(); // ⭐ Lấy script follow
//     }

//     private void Start()
//     {
//         StartCoroutine(SkillLoop());
//     }

//     private IEnumerator SkillLoop()
//     {
//         while (true)
//         {
//             // Boss đang tung skill
//             if (isUsingSkill)
//             {
//                 yield return null;
//                 continue;
//             }

//             // Chưa có target
//             if (target == null)
//             {
//                 yield return null;
//                 continue;
//             }

//             // ⭐ Chỉ dùng skill khi player nằm trong detectRange
//             if (bossFollow != null)
//             {
//                 float dist = Vector2.Distance(transform.position, target.position);

//                 if (dist > bossFollow.detectRange)
//                 {
//                     // Player còn quá xa → boss không dùng skill
//                     yield return null;
//                     continue;
//                 }
//             }

//             // Chọn skill ngẫu nhiên
//             int skill = GetWeightedRandomSkill();
//             lastSkill = skill;

//             isUsingSkill = true;

//             switch (skill)
//             {
//                 case 0:
//                     if (!bossAttack.TryPerformAttack())
//                     {
//                         isUsingSkill = false;
//                         continue;
//                     }
//                     yield return new WaitForSeconds(attackAnimDuration);
//                     break;

//                 case 1:
//                     if (skill1 != null)
//                         yield return StartCoroutine(skill1.CastSkill1());
//                     break;

//                 case 2:
//                     if (skill2 != null)
//                         yield return StartCoroutine(skill2.CastSkill2());
//                     break;
//             }

//             // Cooldown giữa các skill
//             yield return new WaitForSeconds(skillCooldown);

//             // Thêm delay sau skill
//             yield return new WaitForSeconds(postSkillDelay);

//             isUsingSkill = false;
//         }
//     }

//     private int GetWeightedRandomSkill()
//     {
//         while (true)
//         {
//             int total = attackWeight + skill1Weight + skill2Weight;
//             int rand = Random.Range(0, total);

//             int skill = -1;

//             if (rand < attackWeight)
//                 skill = 0;
//             else if (rand < attackWeight + skill1Weight)
//                 skill = 1;
//             else
//                 skill = 2;

//             if (skill != lastSkill)
//                 return skill;
//         }
//     }

//     public bool IsUsingSkill => isUsingSkill;
// }
using UnityEngine;
using System.Collections;

public class BossSkillController : MonoBehaviour
{
    public Transform target;

    [Header("Cooldown Settings")]
    public float skillCooldown = 1.5f;
    [SerializeField] private float postSkillDelay = 2f;

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

    private FrostBossFollow bossFollow;

    private bool isUsingSkill = false;
    private int lastSkill = -1;

    // ⭐ Boss HP
    [Header("Boss Health Reference")]
    [SerializeField] private Health bossHealth;

    // ⭐ Phase chỉ chạy 1 lần
    private bool phaseSkillTriggered = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        bossAttack = GetComponent<BossAttack>();
        bossFollow = GetComponent<FrostBossFollow>();

        if (bossHealth == null)
            bossHealth = GetComponent<Health>();

        // ⭐ Lắng nghe sự kiện HP thay đổi
        bossHealth.OnHealthChanged += OnBossHealthChanged;
    }

    private void Start()
    {
        StartCoroutine(SkillLoop());
    }

    // ⭐⭐⭐ HP GIẢM → CHECK PHASE 50% ⭐⭐⭐
    private void OnBossHealthChanged(float oldHP, float newHP)
    {
        if (!phaseSkillTriggered &&
            newHP <= bossHealth.GetStartingHealth() * 0.5f)
        {
            phaseSkillTriggered = true;

            Debug.Log("🔥 BOSS TRIGGER PHASE 50%!");

            StartCoroutine(SpamSkill1For5Seconds());
        }
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

            // ⭐ Check detect range
            if (bossFollow != null)
            {
                float dist = Vector2.Distance(transform.position, target.position);
                if (dist > bossFollow.detectRange)
                {
                    yield return null;
                    continue;
                }
            }

            // Random skill như cũ
            int skill = GetWeightedRandomSkill();
            lastSkill = skill;

            isUsingSkill = true;

            switch (skill)
            {
                case 0:
                    if (!bossAttack.TryPerformAttack())
                    {
                        isUsingSkill = false;
                        continue;
                    }
                    yield return new WaitForSeconds(attackAnimDuration);
                    break;

                case 1:
                    if (skill1 != null)
                        yield return StartCoroutine(skill1.CastSkill1());
                    break;

                case 2:
                    if (skill2 != null)
                        yield return StartCoroutine(skill2.CastSkill2());
                    break;
            }

            yield return new WaitForSeconds(skillCooldown);
            yield return new WaitForSeconds(postSkillDelay);

            isUsingSkill = false;
        }
    }

    // ⭐⭐⭐ PHASE 50% — BẤT TỬ & SPAM SKILL1 ⭐⭐⭐
    private IEnumerator SpamSkill1For5Seconds()
    {
        Debug.Log("🔥 Boss HP <= 50% → UNSTOPPABLE + SPAM SKILL1!");

        // ⭐ Bất tử
        bool oldInvul = bossHealth.useInvulnerability;
        bossHealth.useInvulnerability = true;

        float timer = 5f;

        while (timer > 0f)
        {
            timer -= 1f;

            if (skill1 != null)
                StartCoroutine(skill1.CastSkill1());

            yield return new WaitForSeconds(1f);
        }

        // ⭐ Hết bất tử
        bossHealth.useInvulnerability = oldInvul;

        Debug.Log("⚠ Boss trở lại trạng thái bình thường!");
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
