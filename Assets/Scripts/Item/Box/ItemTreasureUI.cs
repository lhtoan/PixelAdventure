// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using System.Collections;

// public class ItemTreasureUI : MonoBehaviour
// {
//     [Header("Root UI")]
//     public GameObject root;
//     public Image backgroundDim;

//     [Header("Dim Settings")]
//     public float fadeDuration = 0.3f;
//     public float dimAlpha = 0.5f;

//     [Header("Item UI")]
//     public GameObject coinObj;
//     public TMP_Text txtCoin;

//     public GameObject skillObj;
//     public TMP_Text txtSkill;

//     void Start()
//     {
//         // Reset background dim (alpha = 0)
//         if (backgroundDim != null)
//         {
//             Color c = backgroundDim.color;
//             c.a = 0f;
//             backgroundDim.color = c;
//         }
//     }

//     public void Show(int coin, int skill)
//     {
//         // Bật toàn UI
//         root.SetActive(true);

//         // ⭐ Fade nền mờ giống EndStateUI
//         StartCoroutine(FadeDim(true));

//         // --- COIN ---
//         bool showCoin = coin > 0;
//         coinObj.SetActive(showCoin);
//         txtCoin.gameObject.SetActive(showCoin);

//         if (showCoin)
//             txtCoin.text = "x" + coin;

//         // --- SKILL ---
//         bool showSkill = skill > 0;
//         skillObj.SetActive(showSkill);
//         txtSkill.gameObject.SetActive(showSkill);

//         if (showSkill)
//             txtSkill.text = "x" + skill;
//     }

//     public void Close()
//     {
//         // ⭐ Fade-out giống EndStateUI
//         StartCoroutine(FadeDim(false));

//         root.SetActive(false);
//     }

//     // -------------------------------------------------------
//     // ⭐ Fade alpha của backgroundDim (y chang FadeDim ở EndStateUI)
//     // -------------------------------------------------------
//     private IEnumerator FadeDim(bool fadeIn)
//     {
//         if (backgroundDim == null) yield break;

//         float start = backgroundDim.color.a;
//         float end = fadeIn ? dimAlpha : 0f;

//         float t = 0;
//         Color c = backgroundDim.color;

//         while (t < fadeDuration)
//         {
//             t += Time.deltaTime;
//             c.a = Mathf.Lerp(start, end, t / fadeDuration);
//             backgroundDim.color = c;
//             yield return null;
//         }

//         c.a = end;
//         backgroundDim.color = c;
//     }
// }
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ItemTreasureUI : MonoBehaviour
{
    [Header("Root UI")]
    public GameObject root;
    public Image backgroundDim;

    [Header("Dim Settings")]
    public float fadeDuration = 0.3f;
    public float dimAlpha = 0.5f;

    [Header("Item UI")]
    public GameObject coinObj;
    public TMP_Text txtCoin;

    public GameObject skillObj;
    public TMP_Text txtSkill;

    private bool isShowing = false;  // ⭐ trạng thái đang hiển UI?

    void Start()
    {
        // Reset background dim (alpha = 0)
        if (backgroundDim != null)
        {
            Color c = backgroundDim.color;
            c.a = 0f;
            backgroundDim.color = c;
        }
    }

    void Update()
    {
        // ⭐ Nếu UI đang hiện và click chuột → đóng
        if (isShowing && Input.GetMouseButtonDown(0))
        {
            Close();
        }
    }

    public void Show(int coin, int skill)
    {
        isShowing = true;

        // Bật toàn UI
        root.SetActive(true);

        // Fade background
        StartCoroutine(FadeDim(true));

        // --- COIN ---
        bool showCoin = coin > 0;
        coinObj.SetActive(showCoin);
        txtCoin.gameObject.SetActive(showCoin);

        if (showCoin)
            txtCoin.text = "x" + coin;

        // --- SKILL ---
        bool showSkill = skill > 0;
        skillObj.SetActive(showSkill);
        txtSkill.gameObject.SetActive(showSkill);

        if (showSkill)
            txtSkill.text = "x" + skill;
    }

    public void Close()
    {
        isShowing = false;

        // Fade-out background
        StartCoroutine(FadeDim(false));

        // Tắt UI
        root.SetActive(false);
    }

    // -------------------------------------------------------
    // ⭐ Fade alpha của backgroundDim (như EndStateUI)
    // -------------------------------------------------------
    private IEnumerator FadeDim(bool fadeIn)
    {
        if (backgroundDim == null) yield break;

        float start = backgroundDim.color.a;
        float end = fadeIn ? dimAlpha : 0f;

        float t = 0;
        Color c = backgroundDim.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(start, end, t / fadeDuration);
            backgroundDim.color = c;
            yield return null;
        }

        c.a = end;
        backgroundDim.color = c;
    }
}
