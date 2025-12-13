using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private float volume = 0.4f;

    private bool picked = false;

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
        if (picked) return;
        if (!coll.CompareTag("Player")) return;

        picked = true;

        // Play via AudioManager — dùng clip mặc định coinClip
        if (audioManager != null)
            audioManager.PlaySFX(audioManager.coinClip, volume);

        FindAnyObjectByType<GameManager>().AddScore(1);
        Destroy(gameObject);
    }
}
