using UnityEngine;
using UnityEngine.UI;

public class HpBarEnemy : MonoBehaviour
{
    [SerializeField] private Health enemyHealth;
    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image currentHealthBar;
    [SerializeField] private Transform enemy; // thêm tham chiếu đến Enemy để biết khi nào quay hướng

    private Vector3 originalScale;
    private float maxHealth;

    private void Start()
    {
        if (enemyHealth == null)
            enemyHealth = GetComponentInParent<Health>();

        if (enemy == null && enemyHealth != null)
            enemy = enemyHealth.transform;

        if (enemyHealth != null)
            maxHealth = enemyHealth.GetStartingHealth();

        originalScale = transform.localScale;

        // đảm bảo fill từ trái sang phải
        currentHealthBar.type = Image.Type.Filled;
        currentHealthBar.fillMethod = Image.FillMethod.Horizontal;
        currentHealthBar.fillOrigin = (int)Image.OriginHorizontal.Left;
    }

    private void Update()
    {
        if (enemyHealth == null) return;

        float fill = enemyHealth.currentHealth / maxHealth;
        currentHealthBar.fillAmount = fill;

        // ✅ KHÔNG cho thanh máu bị lật khi enemy đổi hướng
        Vector3 scale = originalScale;

        if (enemy != null && enemy.localScale.x < 0)
        {
            // Nếu enemy quay trái (scale.x âm), thì đảo chiều X ngược lại để giữ thanh đúng hướng
            scale.x = -originalScale.x;
        }

        transform.localScale = scale;
    }
}