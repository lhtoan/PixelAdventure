// using UnityEngine;
// using UnityEngine.UI;

// public class UI_BossHP : MonoBehaviour
// {
//     [Header("Boss Reference")]
//     public Health bossHealth;

//     [Header("HP UI")]
//     public Image totalHp;
//     public Image currentHp;
//     public GameObject hpContainer;   // Object HPBoss

//     private float maxHp;
//     private bool shown = false;

//     private void Start()
//     {
//         if (bossHealth == null)
//         {
//             Debug.LogWarning("❌ UI_BossHP: BossHealth chưa assign!");
//             return;
//         }

//         maxHp = bossHealth.GetStartingHealth();

//         // Ban đầu ẩn thanh HP
//         if (hpContainer != null)
//             hpContainer.SetActive(false);
//     }

//     private void Update()
//     {
//         if (bossHealth == null) return;

//         float hp = bossHealth.currentHealth;

//         // ⭐ Boss mất máu lần đầu → bật UI
//         if (!shown && hp < maxHp)
//         {
//             shown = true;
//             if (hpContainer != null)
//                 hpContainer.SetActive(true);
//         }

//         // ⭐ Cập nhật fill amount
//         if (currentHp != null)
//             currentHp.fillAmount = hp / maxHp;

//         // ⭐ Boss chết → ẩn UI
//         if (hp <= 0)
//         {
//             if (hpContainer != null)
//                 hpContainer.SetActive(false);
//         }
//     }
// }
// using UnityEngine;
// using UnityEngine.UI;

// public class UI_BossHP : MonoBehaviour
// {
//     [Header("Boss Reference")]
//     public Health bossHealth;

//     [Header("HP UI")]
//     public Image totalHp;
//     public Image currentHp;
//     public GameObject hpContainer;   // Object HPBoss

//     private float maxHp;
//     private bool shown = false;

//     private void Start()
//     {
//         if (bossHealth == null)
//         {
//             Debug.LogWarning("❌ UI_BossHP: BossHealth chưa assign!");
//             return;
//         }

//         maxHp = bossHealth.GetStartingHealth();

//         // Ban đầu ẩn thanh HP
//         if (hpContainer != null)
//             hpContainer.SetActive(false);
//     }

//     private void Update()
//     {
//         if (bossHealth == null) return;

//         float hp = bossHealth.currentHealth;

//         // ⭐ Boss mất máu lần đầu → bật UI
//         if (!shown && hp < maxHp)
//         {
//             shown = true;
//             if (hpContainer != null)
//                 hpContainer.SetActive(true);
//         }

//         // ⭐ Cập nhật fill amount
//         if (currentHp != null)
//             currentHp.fillAmount = hp / maxHp;

//         // ⭐ Boss chết → ẩn UI
//         if (hp <= 0)
//         {
//             if (hpContainer != null)
//                 hpContainer.SetActive(false);
//         }
//     }
// }
using UnityEngine;
using UnityEngine.UI;

public class UI_BossHP : MonoBehaviour
{
    [Header("Boss Reference")]
    public Health bossHealth;

    [Header("HP UI")]
    public Image totalHp;
    public Image currentHp;
    public GameObject hpContainer;

    [Header("Camera Control")]
    public CameraMissionController cameraControl;

    [Header("Boss Follow")]
    public FrostBossFollow bossFollow;   // ⭐ Thêm follow để biết khi nào boss phát hiện player

    private float maxHp;
    private bool uiShown = false;

    private void Start()
    {
        if (bossHealth == null)
        {
            Debug.LogWarning("❌ UI_BossHP: BossHealth chưa assign!");
            return;
        }

        if (bossFollow == null)
        {
            bossFollow = GetComponentInParent<FrostBossFollow>();
        }

        maxHp = bossHealth.GetStartingHealth();

        // Ẩn UI lúc đầu
        if (hpContainer != null)
            hpContainer.SetActive(false);

        StartCoroutine(UpdateHP_UI());
    }

    private System.Collections.IEnumerator UpdateHP_UI()
    {
        while (bossHealth != null)
        {
            // ⭐ Boss phát hiện player → bật UI + chuyển camera
            if (!uiShown && bossFollow != null && bossFollow.HasTargetInRange())
            {
                uiShown = true;

                if (hpContainer != null)
                    hpContainer.SetActive(true);

                if (cameraControl != null)
                    cameraControl.ApplyMissionCamera();
            }

            // ⭐ Cập nhật HP UI
            float hp = bossHealth.currentHealth;
            if (currentHp != null)
                currentHp.fillAmount = hp / maxHp;

            // ⭐ Boss chết → tắt UI + trả camera về mặc định
            if (hp <= 0)
            {
                if (hpContainer != null)
                    hpContainer.SetActive(false);

                if (cameraControl != null)
                    cameraControl.ApplyDefaultCamera();

                yield break;
            }

            yield return null;
        }
    }
}
