using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndStateUI : MonoBehaviour
{
    public GameObject winUI;
    public GameObject loseUI;

    [Header("Dim Overlay")]
    public Image dimOverlay;
    public float fadeDuration = 0.3f;
    public float dimAlpha = 0.7f;

    private bool isShowing = false;

    private void Awake()
    {
        // Tắt Win/Lose
        SetActiveRecursive(winUI, false);
        SetActiveRecursive(loseUI, false);

        // ⭐ Đặt alpha của DimOverlay về 0
        if (dimOverlay != null)
        {
            Color c = dimOverlay.color;
            c.a = 0f;
            dimOverlay.color = c;
        }
    }

    private void Update()
    {
        if (isShowing && Input.GetMouseButtonDown(0))
        {
            HideAll();
        }
    }

    public void ShowWin()
    {
        isShowing = true;

        SetActiveRecursive(winUI, true);
        SetActiveRecursive(loseUI, false);

        StartCoroutine(FadeDim(true)); // fade-in
    }

    public void ShowLose()
    {
        isShowing = true;

        SetActiveRecursive(winUI, false);
        SetActiveRecursive(loseUI, true);

        StartCoroutine(FadeDim(true)); // fade-in
    }

    public void HideAll()
    {
        isShowing = false;

        SetActiveRecursive(winUI, false);
        SetActiveRecursive(loseUI, false);

        StartCoroutine(FadeDim(false)); // fade-out
    }

    // -------------------------------------------------------
    // ⭐ Fade alpha của DimOverlay (Image.color)
    // -------------------------------------------------------
    private IEnumerator FadeDim(bool fadeIn)
    {
        if (dimOverlay == null) yield break;

        float start = dimOverlay.color.a;
        float end = fadeIn ? dimAlpha : 0f;

        float t = 0;
        Color c = dimOverlay.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(start, end, t / fadeDuration);
            dimOverlay.color = c;
            yield return null;
        }

        c.a = end;
        dimOverlay.color = c;
    }

    // -------------------------------------------------------
    // Utility: bật/tắt cả parent + child
    // -------------------------------------------------------
    private void SetActiveRecursive(GameObject obj, bool state)
    {
        if (obj == null) return;

        obj.SetActive(state);

        foreach (Transform child in obj.transform)
            child.gameObject.SetActive(state);
    }
}
