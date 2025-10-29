using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
[RequireComponent(typeof(TilemapCollider2D))]
public class HiddenAreaController : MonoBehaviour
{
    [SerializeField] private float fadeSpeed = 1f;      // Tốc độ chuyển màu
    [SerializeField] private float hiddenAlpha = 0f;    // Độ mờ khi bị ẩn (0 = hoàn toàn trong suốt)
    [SerializeField] private float visibleAlpha = 1f;   // Độ mờ khi hiện ra

    private Tilemap tilemap;
    private float targetAlpha;
    private Color currentColor;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        currentColor = tilemap.color;
        targetAlpha = visibleAlpha;
    }

    private void Update()
    {
        // Hiệu ứng mờ dần
        currentColor.a = Mathf.Lerp(currentColor.a, targetAlpha, Time.deltaTime * fadeSpeed);
        tilemap.color = currentColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            targetAlpha = hiddenAlpha; // ẩn hoàn toàn khi player chạm vào
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            targetAlpha = visibleAlpha; // hiện lại khi player rời khỏi
        }
    }
}
