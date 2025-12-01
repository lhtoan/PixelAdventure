// using UnityEngine;
// using UnityEngine.UI;

// public class HealthBar : MonoBehaviour
// {
//     [SerializeField] private Health playerHealth;
//     [SerializeField] private Image totalhealthBar;
//     [SerializeField] private Image currenthealthBar;

//     private void Start()
//     {
//         totalhealthBar.fillAmount = playerHealth.currentHealth / 10;
//     }

//     private void Update()
//     {
//         currenthealthBar.fillAmount = playerHealth.currentHealth / 10;
//     }


// }
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image totalhealthBar;
    [SerializeField] private Image currenthealthBar;

    private const float baseMaxHealth = 10f; // HP tối đa ban đầu (scale UI)

    private void Start()
    {
        UpdateTotalBar();
    }

    private void Update()
    {
        UpdateCurrentBar();
    }

    public void UpdateTotalBar()
    {
        // thanh đen = MAX HP hiện tại / HP tối đa ban đầu
        totalhealthBar.fillAmount = playerHealth.GetStartingHealth() / baseMaxHealth;
    }

    public void UpdateCurrentBar()
    {
        // thanh trắng = current HP / MAX HP hiện tại
        currenthealthBar.fillAmount = playerHealth.currentHealth / baseMaxHealth;
    }
}
