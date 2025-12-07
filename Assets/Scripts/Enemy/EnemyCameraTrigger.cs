using UnityEngine;

public class EnemyHpTriggerCamera : MonoBehaviour
{
    [Header("Enemy Health")]
    public Health enemyHealth;

    [Header("Camera Control")]
    public CameraMissionController cameraControl;

    private float lastHp;
    private bool cameraTriggered = false;

    private void Start()
    {
        if (enemyHealth == null)
        {
            Debug.LogWarning("❌ EnemyHpTriggerCamera: enemyHealth chưa assign!");
            return;
        }

        lastHp = enemyHealth.currentHealth;

        StartCoroutine(CheckHp());
    }

    private System.Collections.IEnumerator CheckHp()
    {
        while (enemyHealth != null)
        {
            float currentHp = enemyHealth.currentHealth;

            // ⭐ Lần đầu enemy bị mất máu → bật camera
            if (!cameraTriggered && currentHp < lastHp)
            {
                cameraTriggered = true;

                if (cameraControl != null)
                    cameraControl.ApplyMissionCamera();
            }

            // ⭐ Enemy chết → trả camera về mặc định
            if (currentHp <= 0)
            {
                if (cameraControl != null)
                    cameraControl.ApplyDefaultCamera();

                yield break;
            }

            lastHp = currentHp;
            yield return null;
        }
    }
}
