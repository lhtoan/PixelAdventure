using UnityEngine;

public class DropItem : MonoBehaviour
{
    [System.Serializable]
    public class DropData
    {
        public GameObject itemPrefab;
        public int minAmount = 1;
        public int maxAmount = 1;
        [Range(0f, 1f)] public float dropChance = 1f;
    }

    public DropData[] drops;

    private Health health;
    private BreakableBox breakableBox;

    private bool dropped = false;

    private void Awake()
    {
        health = GetComponent<Health>();
        breakableBox = GetComponent<BreakableBox>();
    }

    private void Update()
    {
        // ⭐ Kiểm tra chết bằng Health
        if (!dropped && health != null && health.currentHealth <= 0)
        {
            DoDrop();
            dropped = true;
        }

        // ⭐ Kiểm tra chết bằng BreakableBox
        if (!dropped && breakableBox != null && IsBoxBroken())
        {
            DoDrop();
            dropped = true;
        }
    }

    private bool IsBoxBroken()
    {
        // box chết khi collider bị tắt và trigger “break” chạy
        return breakableBox != null &&
               breakableBox.gameObject.GetComponent<Collider2D>().enabled == false;
    }

    private void DoDrop()
    {
        PlayerSkill ps = FindAnyObjectByType<PlayerSkill>();
        foreach (var drop in drops)
        {
            if (Random.value <= drop.dropChance)
            {
                int amount = Random.Range(drop.minAmount, drop.maxAmount + 1);

                if (ps != null)
                    amount = ps.ApplyTreasureBonus(amount);

                for (int i = 0; i < amount; i++)
                    Instantiate(drop.itemPrefab, transform.position, Quaternion.identity);
            }
        }
    }
}
