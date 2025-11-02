using UnityEngine;
using UnityEngine.Tilemaps;

public class HiddenAreaController : MonoBehaviour
{
    private TilemapRenderer tilemapRenderer;

    private void Awake()
    {
        tilemapRenderer = GetComponent<TilemapRenderer>();
    }

    public void SetHidden(bool hidden)
    {
        tilemapRenderer.enabled = !hidden;
    }
}
