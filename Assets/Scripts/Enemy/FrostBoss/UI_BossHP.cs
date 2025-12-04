using UnityEngine;
using UnityEngine.UI;

public class UI_BossHP : MonoBehaviour
{
    [Header("Boss Reference")]
    public Health bossHealth;

    [Header("HP UI")]
    public Image totalHp;
    public Image currentHp;
    public GameObject hpContainer;   // Object HPBoss

    private float maxHp;
    private bool shown = false;

    private void Start()
    {
        if (bossHealth == null)
        {
            Debug.LogWarning("❌ UI_BossHP: BossHealth chưa assign!");
            return;
        }

        maxHp = bossHealth.GetStartingHealth();

        // Ban đầu ẩn thanh HP
        if (hpContainer != null)
            hpContainer.SetActive(false);
    }

    private void Update()
    {
        if (bossHealth == null) return;

        float hp = bossHealth.currentHealth;

        // ⭐ Boss mất máu lần đầu → bật UI
        if (!shown && hp < maxHp)
        {
            shown = true;
            if (hpContainer != null)
                hpContainer.SetActive(true);
        }

        // ⭐ Cập nhật fill amount
        if (currentHp != null)
            currentHp.fillAmount = hp / maxHp;

        // ⭐ Boss chết → ẩn UI
        if (hp <= 0)
        {
            if (hpContainer != null)
                hpContainer.SetActive(false);
        }
    }
}
