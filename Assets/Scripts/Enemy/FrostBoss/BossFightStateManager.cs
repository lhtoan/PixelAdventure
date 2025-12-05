using UnityEngine;

public class BossFightStateManager : MonoBehaviour
{
    [Header("References")]
    public Health bossHealth;      // kéo Boss.FrostBoss → Health
    public Health playerHealth;    // kéo Player → Health
    public EndStateUI endUI;       // kéo object EndBoss vào đây

    private bool ended = false;

    void Update()
    {
        if (ended) return;

        if (bossHealth == null || playerHealth == null || endUI == null)
        {
            Debug.LogWarning("❌ Missing reference in BossFightStateManager");
            return;
        }

        // ⭐ Boss chết → WIN
        if (bossHealth.currentHealth <= 0)
        {
            ended = true;
            endUI.ShowWin();
            return;
        }

        // ⭐ Player chết trước boss → LOSE
        // if (playerHealth.currentHealth <= 0)
        // {
        //     ended = true;
        //     endUI.ShowLose();
        //     return;
        // }
    }
}
