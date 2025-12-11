// using UnityEngine;

// public class Coin : MonoBehaviour
// {
//     private bool picked = false;

//     private void OnTriggerEnter2D(Collider2D coll)
//     {
//         if (picked) return;         // ⭐ Ngăn +2 khi 2 collider của Player chạm cùng lúc
//         if (!coll.CompareTag("Player")) return;

//         picked = true;              // ⭐ Đánh dấu coin đã nhặt

//         FindAnyObjectByType<GameManager>().AddScore(1);
//         Destroy(gameObject);
//     }
// }
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private AudioClip pickSound;   // ⭐ Kéo file âm vào đây
    [SerializeField] private float volume = 1f;

    private bool picked = false;

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (picked) return;
        if (!coll.CompareTag("Player")) return;

        picked = true;

        // ⭐ Phát âm thanh nhặt coin (không bị cắt khi coin bị destroy)
        PlayPickupSound();

        // ⭐ Cộng điểm
        FindAnyObjectByType<GameManager>().AddScore(1);

        // ⭐ Xóa coin
        Destroy(gameObject);
    }

    private void PlayPickupSound()
    {
        if (pickSound == null) return;

        // Tạo object âm thanh tạm thời
        GameObject audioObj = new GameObject("CoinSound");
        AudioSource audioSource = audioObj.AddComponent<AudioSource>();

        audioSource.clip = pickSound;
        audioSource.volume = volume;
        audioSource.Play();

        // Tự hủy sau khi âm thanh kết thúc
        Destroy(audioObj, pickSound.length);
    }
}
