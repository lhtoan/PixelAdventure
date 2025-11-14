using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill_E_Fire : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private List<GameObject> fireballs;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private PlayerStamina playerStamina;   //stamina
    [SerializeField] private PlayerSkill playerSkill;
    [SerializeField] private UI_SkillBarIcon skillBarIcon;   // UI icon của skill này



    [Header("Settings")]
    [SerializeField] private float shootSpeed = 6f;
    [SerializeField] private float damage = 8f;
    [SerializeField] private float cooldown = 4f;
    [SerializeField] private float spreadRadius = 1f;
    [SerializeField] private float staminaCost = 4f;        //stamina cost

    [Header("Multi-Circle Settings")]
    [SerializeField] private int fireballPerCircle = 8;
    [SerializeField] private float delayBetweenCircles = 1f;
    [SerializeField] private float secondRingOffsetAngle = 22.5f;

    private bool isOnCooldown = false;
    private int nextFireballIndex = 0;

    private void Awake()
    {
        playerAttack = GetComponentInParent<PlayerAttack>();
        playerStamina = GetComponentInParent<PlayerStamina>(); // ⭐ auto lấy stamina từ Player
        playerSkill = GetComponentInParent<PlayerSkill>();
        if (skillBarIcon == null)
            Debug.LogWarning("⚠ Fire_E missing skillBarIcon reference in Inspector!");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isOnCooldown)
        {
            // ⭐ CHECK MỞ KHÓA SKILL
            if (!playerSkill.IsSkillUnlocked(PlayerSkill.SkillType.Fire_E))
            {
                Debug.Log("❌ Skill E Fire chưa mở khóa!");
                return;
            }

            // ⭐ CHECK Stamina + đúng hệ
            if (playerAttack != null &&
                playerAttack.CurrentElement == PlayerAttack.Element.Fire &&
                playerStamina != null &&
                playerStamina.CanUse(staminaCost))
            {
                StartCoroutine(DoubleFireBurst());
            }
        }
    }

    private IEnumerator DoubleFireBurst()
    {
        isOnCooldown = true;

        if (skillBarIcon != null)
        {
            skillBarIcon.StartCooldown(cooldown);
        }

        // ⭐ TRỪ STAMINA khi dùng chiêu
        playerStamina.Use(staminaCost);

        // 🔥 Bắn vòng 1
        CastFireCircle(0f);

        // ⏱ Chờ để bắn vòng 2
        yield return new WaitForSeconds(delayBetweenCircles);

        // 🔥 Bắn vòng 2 xoay lệch
        CastFireCircle(secondRingOffsetAngle);

        // 🕐 Hồi chiêu
        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }

    private void CastFireCircle(float angleOffset)
    {
        if (fireballs.Count == 0) return;

        float angleStep = 360f / fireballPerCircle;
        int count = Mathf.Min(fireballPerCircle, fireballs.Count);

        for (int i = 0; i < count; i++)
        {
            GameObject fb = fireballs[nextFireballIndex];
            nextFireballIndex = (nextFireballIndex + 1) % fireballs.Count;

            if (fb == null) continue;

            float angle = (angleStep * i + angleOffset) * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector3 spawnPos = firePoint.position + dir * spreadRadius;

            fb.transform.position = spawnPos;
            fb.transform.right = dir;
            fb.SetActive(true);

            Rigidbody2D rb = fb.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            Fireball script = fb.GetComponent<Fireball>();
            if (script != null)
                script.Launch(dir, shootSpeed, damage);
        }
    }
}
