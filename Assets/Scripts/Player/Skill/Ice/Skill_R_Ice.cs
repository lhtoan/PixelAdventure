using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill_R_Ice : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform icePoint;                // điểm spawn cột băng
    [SerializeField] private List<GameObject> iceSpikePool;     // pool các cột băng

    [Header("Skill Settings")]
    [SerializeField] private float cooldown = 6f;
    [SerializeField] private float staminaCost = 5f;
    [SerializeField] private float spikeDistance = 1.8f;        // khoảng cách giữa các cột
    [SerializeField] private float spikeLifetime = 1.3f;        // tồn tại bao lâu

    private bool isOnCooldown = false;

    private PlayerAttack playerAttack;
    private PlayerStamina playerStamina;
    private PlayerSkill playerSkill;

    private void Awake()
    {
        playerAttack = GetComponentInParent<PlayerAttack>();
        playerStamina = GetComponentInParent<PlayerStamina>();
        playerSkill = GetComponentInParent<PlayerSkill>();

        // đảm bảo tất cả spike trong pool đều tắt lúc start
        for (int i = 0; i < iceSpikePool.Count; i++)
        {
            if (iceSpikePool[i] != null)
                iceSpikePool[i].SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) &&
            !isOnCooldown &&
            playerAttack.CurrentElement == PlayerAttack.Element.Ice)
        {
            TryCast();
        }
    }

    private void TryCast()
    {
        // CHECK SKILL UNLOCK
        if (!playerSkill.IsSkillUnlocked(PlayerSkill.SkillType.Ice_R))
        {
            Debug.Log("❌ Skill Ice R chưa mở khóa!");
            return;
        }

        // CHECK stamina
        if (!playerStamina.CanUse(staminaCost))
        {
            Debug.Log("❌ Không đủ stamina để dùng Ice R!");
            return;
        }

        // Trừ stamina
        playerStamina.Use(staminaCost);

        StartCoroutine(CastIceSpikes());
    }

    private IEnumerator CastIceSpikes()
    {
        isOnCooldown = true;

        float direction = Mathf.Sign(playerAttack.transform.localScale.x);

        // 3 cột băng liên tục
        for (int i = 0; i < iceSpikePool.Count; i++)
        {
            GameObject spike = iceSpikePool[i];

            if (spike == null)
                continue;

            Vector3 spawnPos = icePoint.position + new Vector3(i * spikeDistance * direction, 0f, 0f);

            spike.transform.position = spawnPos;

            // xoay đúng hướng player
            Vector3 scale = spike.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * direction;
            spike.transform.localScale = scale;

            spike.SetActive(true);

            StartCoroutine(DeactivateSpike(spike, spikeLifetime));

            yield return new WaitForSeconds(0.3f); // delay nhẹ giữa mỗi spike
        }

        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }

    private IEnumerator DeactivateSpike(GameObject spike, float delay)
    {
        yield return new WaitForSeconds(delay);
        spike.SetActive(false);
    }
}
