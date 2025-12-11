using UnityEngine;

public class SkillPointPickup : MonoBehaviour
{
    private bool picked = false;   // ⭐ chống nhặt 2 lần
    [SerializeField] private AudioManager audioManager;

    private void Awake()
    {
        if (audioManager == null)
        {
            GameObject gm = GameObject.FindGameObjectWithTag("GameManager");
            if (gm != null) audioManager = gm.GetComponent<AudioManager>();
        }
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (picked) return; // nếu đã nhặt rồi → thoát
        if (!coll.CompareTag("Player")) return;

        picked = true; // đánh dấu nhặt rồi

        if (audioManager != null && audioManager.skillpointClip != null)
            audioManager.PlaySFX(audioManager.skillpointClip, 1);

        FindAnyObjectByType<GameManager>().AddSkillPoint(1);

        Destroy(gameObject);
    }
}
