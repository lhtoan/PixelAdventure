using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 10f;
    [SerializeField] private float regenRate = 2f;

    private float currentStamina;

    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;

    private void Awake()
    {
        currentStamina = maxStamina;
    }

    private void Update()
    {
        RegenerateStamina();
    }

    // --- Hồi dần ---
    private void RegenerateStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += regenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
    }

    // --- Kiểm tra ---
    public bool CanUse(float cost)
    {
        return currentStamina >= cost;
    }

    // --- Tiêu hao ---
    public void Use(float cost)
    {
        currentStamina -= cost;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

    // --- Hồi ngay lập tức (nếu cần) ---
    public void Restore(float amount)
    {
        currentStamina += amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

    public void IncreaseMaxStamina(float amount)
    {
        maxStamina += amount;
        currentStamina += amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

}
