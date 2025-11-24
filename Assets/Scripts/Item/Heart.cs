using UnityEngine;

public class Heart : MonoBehaviour
{
    private bool picked = false;

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (picked) return;                 // ⭐ Chống +2
        if (!coll.CompareTag("Player")) return;

        picked = true;                      // ⭐ Đánh dấu đã nhặt

        Health hp = coll.GetComponent<Health>();
        if (hp != null)
            hp.AddHealth(1);

        Destroy(gameObject);                // ⭐ Hủy Heart
    }
}
