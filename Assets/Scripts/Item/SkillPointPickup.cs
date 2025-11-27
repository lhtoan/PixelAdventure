using UnityEngine;

public class SkillPointPickup : MonoBehaviour
{
    private bool picked = false;   // ⭐ chống nhặt 2 lần

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (picked) return; // nếu đã nhặt rồi → thoát
        if (!coll.CompareTag("Player")) return;

        picked = true; // đánh dấu nhặt rồi

        FindAnyObjectByType<GameManager>().AddSkillPoint(1);

        Destroy(gameObject);
    }
}
