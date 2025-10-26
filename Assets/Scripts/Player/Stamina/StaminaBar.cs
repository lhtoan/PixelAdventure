using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private PlayerController player; // Gắn PlayerController có mana vào đây
    [SerializeField] private Image totalStaminaBar;   // Thanh stamina tổng (màu nhạt)
    [SerializeField] private Image currentStaminaBar; // Thanh stamina hiện tại (màu đậm)

    private void Start()
    {
        if (player != null)
        {
            totalStaminaBar.fillAmount = 1f; // luôn đầy 100%
        }
    }

    private void Update()
    {
        if (player != null)
        {
            float ratio = player.CurrentMana / player.MaxMana;
            currentStaminaBar.fillAmount = ratio;
        }
    }
}
