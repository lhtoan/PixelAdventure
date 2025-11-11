using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill_R_Fire : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform R_FirePoint;
    [SerializeField] private List<GameObject> tornadoPool;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private PlayerStamina playerStamina;
    [SerializeField] private PlayerSkill playerSkill;   // ⭐ NEW

    [Header("Skill Settings")]
    [SerializeField] private float cooldown = 8f;
    [SerializeField] private float staminaCost = 5f;

    private bool isOnCooldown = false;

    private void Awake()
    {
        playerAttack = GetComponentInParent<PlayerAttack>();
        playerStamina = GetComponentInParent<PlayerStamina>();
        playerSkill = GetComponentInParent<PlayerSkill>(); // ⭐ auto lấy từ Player

        // Ensure all pool objects start inactive
        for (int i = 0; i < tornadoPool.Count; i++)
        {
            if (tornadoPool[i] != null && tornadoPool[i].activeSelf)
                tornadoPool[i].SetActive(false);
        }
    }

    private void Update()
    {
        // ⭐ CHƯA MỞ KHÓA KHÔNG CHO DÙNG
        if (!playerSkill.IsSkillUnlocked(PlayerSkill.SkillType.Fire_R))
        {
            // Debug.Log("❌ Skill R Fire chưa mở khóa!");
            return;
        }

        if (!isOnCooldown &&
            Input.GetKeyDown(KeyCode.R) &&
            playerAttack != null &&
            playerAttack.CurrentElement == PlayerAttack.Element.Fire &&
            playerStamina.CanUse(staminaCost))
        {
            SpawnOrb();
        }
    }

    private void SpawnOrb()
    {
        isOnCooldown = true;

        // ⭐ TRỪ STAMINA
        playerStamina.Use(staminaCost);

        if (R_FirePoint == null)
        {
            Debug.LogError("[Skill_R_Fire] R_FirePoint is NOT assigned!");
            isOnCooldown = false;
            return;
        }

        if (tornadoPool == null || tornadoPool.Count == 0)
        {
            Debug.LogError("[Skill_R_Fire] tornadoPool is empty.");
            isOnCooldown = false;
            return;
        }

        Vector3 spawnPos = R_FirePoint.position;

        // tìm object inactive trong pool
        GameObject orbObj = null;

        for (int i = 0; i < tornadoPool.Count; i++)
        {
            if (!tornadoPool[i].activeSelf)
            {
                orbObj = tornadoPool[i];
                break;
            }
        }

        // nếu tất cả đều active → reset object đầu tiên
        if (orbObj == null)
        {
            orbObj = tornadoPool[0];
            orbObj.SetActive(false);
        }

        // SPAWN NGAY
        orbObj.transform.position = spawnPos;
        orbObj.transform.rotation = Quaternion.identity;

        orbObj.SetActive(false);
        orbObj.SetActive(true);

        FireChasingOrb orb = orbObj.GetComponent<FireChasingOrb>();

        if (orb != null)
        {
            orb.Initialize(Vector2.zero); // orb tự tìm enemy
        }
        else
        {
            Debug.LogError("[Skill_R_Fire] Prefab does NOT contain FireChasingOrb script!");
        }

        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }
}
