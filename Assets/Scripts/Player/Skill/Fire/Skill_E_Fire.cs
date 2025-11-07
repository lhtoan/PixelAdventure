using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill_E_Fire : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private List<GameObject> fireballs; // pool chứa tất cả fireball
    [SerializeField] private PlayerAttack playerAttack;

    [Header("Settings")]
    [SerializeField] private float shootSpeed = 6f;
    [SerializeField] private float damage = 8f;
    [SerializeField] private float cooldown = 4f;
    [SerializeField] private float spreadRadius = 1f;

    [Header("Multi-Circle Settings")]
    [SerializeField] private int fireballPerCircle = 8; // số viên mỗi vòng
    [SerializeField] private float delayBetweenCircles = 1f; // thời gian giữa 2 vòng
    [SerializeField] private float secondRingOffsetAngle = 22.5f; // góc lệch của vòng 2 (độ)

    private bool isOnCooldown = false;
    private int nextFireballIndex = 0; // dùng để xoay vòng qua pool

    private void Awake()
    {
        playerAttack = GetComponentInParent<PlayerAttack>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isOnCooldown)
        {
            if (playerAttack != null && playerAttack.CurrentElement == PlayerAttack.Element.Fire)
            {
                StartCoroutine(DoubleFireBurst());
            }
        }
    }

    private IEnumerator DoubleFireBurst()
    {
        isOnCooldown = true;

        // 🔥 Bắn vòng 1
        CastFireCircle(0f);

        // ⏱ Chờ 1 giây
        yield return new WaitForSeconds(delayBetweenCircles);

        // 🔥 Bắn vòng 2 (xoay lệch)
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
            // 🔄 Lấy fireball từ pool theo index, xoay vòng nếu hết
            GameObject fb = fireballs[nextFireballIndex];
            nextFireballIndex = (nextFireballIndex + 1) % fireballs.Count;

            if (fb == null) continue;

            // Tính góc bắn có offset
            float angle = (angleStep * i + angleOffset) * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector3 spawnPos = firePoint.position + dir * spreadRadius;

            // Cấu hình viên fireball
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
