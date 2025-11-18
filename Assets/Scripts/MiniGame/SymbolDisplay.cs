using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SymbolDisplay : MonoBehaviour
{
    public RectTransform container;     // SymbolContainer
    public GameObject iconPrefab;       // Prefab của 1 icon
    private List<GameObject> spawnedIcons = new List<GameObject>();

    public void SetSymbols(DrawSymbol[] symbols)
    {
        // Xóa icon cũ
        foreach (Transform child in container)
            Destroy(child.gameObject);

        // Spawn icon mới
        foreach (DrawSymbol sym in symbols)
        {
            GameObject icon = Instantiate(iconPrefab, container);

            // 🔥 prefab inactive -> phải bật
            icon.SetActive(true);

            Image img = icon.GetComponent<Image>();
            img.sprite = GetSprite(sym);
        }
    }


    private Sprite GetSprite(DrawSymbol s)
    {
        return s switch
        {
            DrawSymbol.LineVertical => lineVertical,
            DrawSymbol.LineHorizontal => lineHorizontal,
            DrawSymbol.VShape => vShape,
            DrawSymbol.AShape => aShape,
            DrawSymbol.Circle => circle,
            DrawSymbol.Lightning => lightning,
            DrawSymbol.Spiral => spiral,
            DrawSymbol.ZShape => zShape,
            _ => null
        };
    }

    // Các sprite assign tại Inspector
    public Sprite lineVertical;
    public Sprite lineHorizontal;
    public Sprite vShape;
    public Sprite aShape;
    public Sprite circle;
    public Sprite lightning;
    public Sprite spiral;
    public Sprite zShape;
}
