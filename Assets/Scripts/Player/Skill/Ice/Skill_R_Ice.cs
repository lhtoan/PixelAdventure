using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill_R_Ice : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform icePoint;
    [SerializeField] private List<GameObject> iceSpikePool;

    [Header("Skill Settings")]
    [SerializeField] private float cooldown = 6f;
    [SerializeField] private float staminaCost = 5f;
    [SerializeField] private float spikeDistance = 1.8f;
    [SerializeField] private float spikeLifetime = 1.3f;

    private bool isOnCooldown = false;

    private PlayerAttack playerAttack;
    private PlayerStamina playerStamina;
    private PlayerSkill playerSkill;
    [SerializeField] private UI_SkillBarIcon skillBarIcon;

    private void Awake()
    {
        playerAttack = GetComponentInParent<PlayerAttack>();
        playerStamina = GetComponentInParent<PlayerStamina>();
        playerSkill = GetComponentInParent<PlayerSkill>();

        foreach (var spike in iceSpikePool)
            if (spike != null) spike.SetActive(false);
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
        if (!playerSkill.IsSkillUnlocked(PlayerSkill.SkillType.Ice_R))
            return;

        if (!playerStamina.CanUse(staminaCost))
            return;

        playerStamina.Use(staminaCost);

        StartCoroutine(CastIceSpikes());
    }

    private IEnumerator CastIceSpikes()
    {
        isOnCooldown = true;

        if (skillBarIcon != null)
            skillBarIcon.StartCooldown(cooldown);

        float direction = Mathf.Sign(playerAttack.transform.localScale.x);
        Vector3 castOrigin = icePoint.position;

        for (int i = 0; i < iceSpikePool.Count; i++)
        {
            GameObject spike = iceSpikePool[i];
            if (spike == null) continue;

            Vector3 spawnPos = castOrigin + new Vector3(i * spikeDistance * direction, 0f, 0f);
            spike.transform.position = spawnPos;

            Vector3 scale = spike.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * direction;
            spike.transform.localScale = scale;

            // ⭐ TRUYỀN ATTACKER VÀO ICESPIKE
            IceSpike spikeComp = spike.GetComponent<IceSpike>();
            if (spikeComp != null)
                spikeComp.SetAttacker(playerAttack);

            spike.SetActive(true);

            StartCoroutine(DeactivateSpike(spike, spikeLifetime));

            yield return new WaitForSeconds(0.3f);
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
