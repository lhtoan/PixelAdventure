using UnityEngine;

public class DeleteItem : MonoBehaviour
{
    [Header("Objects to Toggle")]
    public GameObject deleteItem;   // ⭐ Object sẽ ẩn khi chết
    public GameObject item;         // ⭐ Object sẽ hiện khi chết

    [Header("Toggle Settings")]
    public bool hideDeleteItem = true;  // Ẩn deleteItem khi chết?
    public bool showItem = false;       // Hiện item khi chết?

    private Health health;
    private bool hasTriggered = false;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void Update()
    {
        if (hasTriggered) return;
        if (health == null) return;

        // Khi máu về 0 → kích hoạt
        if (health.currentHealth <= 0)
        {
            hasTriggered = true;

            // ⭐ ẨN deleteItem
            if (hideDeleteItem && deleteItem != null)
                deleteItem.SetActive(false);

            // ⭐ HIỆN item
            if (showItem && item != null)
                item.SetActive(true);
        }
    }
}
