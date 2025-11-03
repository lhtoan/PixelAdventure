using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private PlayerStamina playerStamina; // 🔹 Gắn script PlayerStamina
    [SerializeField] private Image totalStaminaBar;       // Thanh tổng (màu nền)
    [SerializeField] private Image currentStaminaBar;     // Thanh hiện tại (màu đậm)

    private void Start()
    {
        if (playerStamina != null && totalStaminaBar != null)
        {
            totalStaminaBar.fillAmount = 1f; // luôn đầy 100%
        }
    }

    private void Update()
    {
        if (playerStamina != null && currentStaminaBar != null)
        {
            float ratio = playerStamina.CurrentStamina / playerStamina.MaxStamina;
            currentStaminaBar.fillAmount = ratio;
        }
    }
}
