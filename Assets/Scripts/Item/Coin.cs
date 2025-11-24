using UnityEngine;

public class Coin : MonoBehaviour
{
    private bool picked = false;

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (picked) return;         // ⭐ Ngăn +2 khi 2 collider của Player chạm cùng lúc
        if (!coll.CompareTag("Player")) return;

        picked = true;              // ⭐ Đánh dấu coin đã nhặt

        FindAnyObjectByType<GameManager>().AddScore(1);
        Destroy(gameObject);
    }
}
