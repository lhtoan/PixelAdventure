using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class SymbolDisplay : MonoBehaviour
{
    public RectTransform container;     // SymbolContainer
    public GameObject iconPrefab;       // Prefab của 1 icon
    private List<GameObject> spawnedIcons = new List<GameObject>();

    public void SetSymbols(DrawSymbol[] symbols)
    {
        // Ẩn icon cũ
        foreach (var icon in spawnedIcons)
        {
            if (icon != null)
                icon.SetActive(false);
        }

        spawnedIcons.Clear();

        // Spawn icon mới
        foreach (DrawSymbol sym in symbols)
        {
            GameObject icon = Instantiate(iconPrefab, container);
            icon.SetActive(true);

            Image img = icon.GetComponent<Image>();
            img.color = Color.white; // reset màu
            img.sprite = GetSprite(sym);

            spawnedIcons.Add(icon);
        }
    }


    public void HideIndex(int index)
    {
        if (index < 0 || index >= spawnedIcons.Count) return;

        GameObject icon = spawnedIcons[index];
        StartCoroutine(FadeAndShrink(icon));
    }

    private IEnumerator FadeAndShrink(GameObject icon)
    {
        Image img = icon?.GetComponent<Image>();
        if (img == null) yield break;

        float t = 0;
        Vector3 startScale = icon.transform.localScale;

        while (t < 0.2f)
        {
            if (icon == null || img == null) yield break;

            t += Time.unscaledDeltaTime;

            float a = 1f - (t / 0.2f);
            img.color = new Color(img.color.r, img.color.g, img.color.b, a);

            icon.transform.localScale = startScale * (1f - t / 0.2f);

            yield return null;
        }

        if (icon != null)
            icon.SetActive(false);
    }


    public void ShakeWrong(int index)
    {
        if (index < 0 || index >= spawnedIcons.Count) return;
        StartCoroutine(ShakeAndRed(spawnedIcons[index]));
    }

    private IEnumerator ShakeAndRed(GameObject icon)
    {
        Image img = icon.GetComponent<Image>();
        Color original = img.color;

        // chuyển đỏ
        img.color = Color.red;

        Vector3 start = icon.transform.localPosition;

        // rung 3 lần
        for (int i = 0; i < 4; i++)
        {
            icon.transform.localPosition = start + new Vector3(UnityEngine.Random.Range(-5, 5), 0, 0);
            yield return new WaitForSecondsRealtime(0.03f);
        }

        // reset
        icon.transform.localPosition = start;
        img.color = original;
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
