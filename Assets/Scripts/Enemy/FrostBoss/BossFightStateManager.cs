using System.Collections;
using UnityEngine;

public class BossFightStateManager : MonoBehaviour
{
    public Health bossHealth;
    public Health playerHealth;
    public MainMenuUI menuUI;   // ⭐ đổi thành MainMenuUI

    private bool ended = false;

    void Update()
    {
        if (ended) return;

        if (bossHealth.currentHealth <= 0)
        {
            ended = true;

            // ⭐ KHÔNG show Win UI ngay lập tức
            StartCoroutine(HandleBossWin());
        }
    }

    private IEnumerator HandleBossWin()
    {
        // 1) Chờ camera về default (CameraMissionController đang chạy animation)
        yield return new WaitForSecondsRealtime(0); // transitionDuration của bạn = 1s → chờ ~80%

        // 2) Hiện Win UI
        menuUI.ShowBossWin();
    }


}
