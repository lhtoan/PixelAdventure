using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill_R_Ice : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Transform rPoint;                  // R_Ice_Point — điểm bắn đầu tiên
    [SerializeField] private List<GameObject> iceSpikes;        // Danh sách cột băng có sẵn (R, R (1), R (2))
    [SerializeField] private float distanceBetweenSpikes = 2f;  // Khoảng cách giữa các cột
    [SerializeField] private float delayBetweenSpikes = 0.4f;   // Độ trễ giữa từng cột
    [SerializeField] private float spikeLifetime = 1.5f;        // Tồn tại bao lâu

    [Header("Cooldown Settings")]
    [SerializeField] private float skillCooldown = 5f;          // Thời gian hồi chiêu
    private float cooldownTimer = Mathf.Infinity;

    [Header("References")]
    [SerializeField] private PlayerAttack playerAttack;

    private bool isCasting = false;

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.R) && !isCasting && cooldownTimer >= skillCooldown)
        {
            if (playerAttack != null && playerAttack.CurrentElement == PlayerAttack.Element.Ice)
            {
                StartCoroutine(CastSkill());
            }
            else
            {
                // Debug.Log("❌ Không đúng hệ để dùng chiêu R!");
            }
        }
    }

    private IEnumerator CastSkill()
    {
        isCasting = true;
        cooldownTimer = 0f; // reset cooldown

        Debug.Log("🧊 Kích hoạt chiêu R - Ice Spikes!");

        // ✅ Lấy hướng player tại thời điểm cast
        float direction = Mathf.Sign(playerAttack.transform.localScale.x);

        // ✅ Vị trí bắt đầu
        Vector3 startPos = rPoint.position;

        // ✅ Kích hoạt lần lượt từng spike trong danh sách
        for (int i = 0; i < iceSpikes.Count; i++)
        {
            GameObject spike = iceSpikes[i];
            if (spike == null) continue;

            // Tính vị trí cho spike này
            Vector3 spawnPos = startPos + new Vector3(i * distanceBetweenSpikes * direction, 0f, 0f);

            // Đặt vị trí và bật lên
            spike.transform.position = spawnPos;
            spike.SetActive(true);

            // Hướng đúng chiều player
            Vector3 scale = spike.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * direction;
            spike.transform.localScale = scale;

            // Hủy sau thời gian tồn tại
            StartCoroutine(DeactivateAfterDelay(spike, spikeLifetime));

            // Chờ trước khi spike tiếp theo bật lên
            yield return new WaitForSeconds(delayBetweenSpikes);
        }

        isCasting = false;
    }

    private IEnumerator DeactivateAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
            obj.SetActive(false);
    }
}
