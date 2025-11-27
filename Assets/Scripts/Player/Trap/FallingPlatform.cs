// using System.Collections;
// using UnityEngine;

// public class FallingPlatform : MonoBehaviour
// {
//     [SerializeField] private float fallDelay = 1f;
//     [SerializeField] private float destroyDelay = 2f;

//     private bool falling = false;

//     [SerializeField] private Rigidbody2D rb;

//     private void OnCollisionEnter2D(Collision2D collision)
//     {
//         // Avoid calling the coroutine multiple times if it's already been called (falling)
//         if (falling)
//             return;

//         // If the player landed on the platform, start falling
//         if (collision.transform.tag == "Player")
//         {
//             StartCoroutine(StartFall());
//         }
//     }

//     private IEnumerator StartFall()
//     {
//         falling = true;

//         // Wait for a few seconds before dropping
//         yield return new WaitForSeconds(fallDelay);

//         // Enable rigidbody and destroy after a few seconds
//         rb.bodyType = RigidbodyType2D.Dynamic;
//         Destroy(gameObject, destroyDelay);
//     }
// }
using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float fallDelay = 1f;
    [SerializeField] private float respawnTime = 3f;

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject platformVisual; // sprite / toàn bộ model

    private Vector3 startPos;
    private Quaternion startRot;

    private bool falling = false;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (platformVisual == null) platformVisual = transform.GetChild(0).gameObject;

        startPos = transform.position;
        startRot = transform.rotation;

        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (falling) return;

        if (collision.transform.CompareTag("Player"))
        {
            StartCoroutine(FallRoutine());
        }
    }

    private IEnumerator FallRoutine()
    {
        falling = true;

        yield return new WaitForSeconds(fallDelay);

        // ⭐ Bắt đầu rơi
        rb.bodyType = RigidbodyType2D.Dynamic;

        // ⭐ Chờ rơi xong rồi respawn
        yield return new WaitForSeconds(respawnTime);

        // ⭐ Respawn platform
        RespawnPlatform();
    }

    private void RespawnPlatform()
    {
        // 1. Tắt Rigidbody (không rơi)
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // 2. Đưa platform về vị trí ban đầu
        transform.position = startPos;
        transform.rotation = startRot;

        // 3. Hiện lại platform (nếu bạn ẩn sprite lúc rơi)
        if (platformVisual != null)
            platformVisual.SetActive(true);

        // Sẵn sàng rơi lại lần nữa
        falling = false;
    }
}


//Naitosaysua