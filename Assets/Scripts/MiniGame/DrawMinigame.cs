using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DrawMinigame : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup canvasGroup;
    public Image dimBackground;
    public float fadeDuration = 0.25f;
    public bool pauseGameWhenOpen = true;

    [Header("Player Control")]
    private PlayerController playerController;
    private PlayerAttack playerAttack;

    private bool isOpen = false;
    private Coroutine fadeUIRoutine;
    private Coroutine fadeDimRoutine;

    [Header("Symbol")]
    public DrawSymbol currentSymbol;
    public SymbolDisplay symbolDisplay;

    private void Awake()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerController = playerObj.GetComponent<PlayerController>();
            playerAttack = playerObj.GetComponent<PlayerAttack>();
        }
    }

    private void Start()
    {
        // Ẩn UI hoàn toàn lúc đầu
        SetCanvasVisible(false, true);
        SetDimVisible(false, true);

        // Tắt SymbolTarget lúc đầu
        if (symbolDisplay != null)
            symbolDisplay.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            ToggleMinigame();

        if (Input.GetKeyDown(KeyCode.Escape) && isOpen)
            CloseMinigame();
    }

    // -----------------------------------------------------------
    // TOGGLE Minigame
    // -----------------------------------------------------------
    private void ToggleMinigame()
    {
        isOpen = !isOpen;

        SetCanvasVisible(isOpen);
        SetDimVisible(isOpen);
        LockPlayerControls(isOpen);

        if (pauseGameWhenOpen)
            Time.timeScale = isOpen ? 0f : 1f;

        if (isOpen)
        {
            symbolDisplay.gameObject.SetActive(true);
            OpenMinigame(); // random ký hiệu
        }
        else
        {
            symbolDisplay.gameObject.SetActive(false);
        }

        Debug.Log(isOpen ? "🎮 Minigame mở!" : "❌ Minigame tắt!");
    }

    // -----------------------------------------------------------
    // CLOSE Minigame
    // -----------------------------------------------------------
    private void CloseMinigame()
    {
        isOpen = false;

        SetCanvasVisible(false);
        SetDimVisible(false);
        LockPlayerControls(false);

        if (pauseGameWhenOpen)
            Time.timeScale = 1f;

        symbolDisplay.gameObject.SetActive(false);

        Debug.Log("❌ Minigame đã đóng.");
    }

    // -----------------------------------------------------------
    // LOCK PLAYER INPUT
    // -----------------------------------------------------------
    private void LockPlayerControls(bool locked)
    {
        if (playerController != null)
            playerController.enabled = !locked;

        if (playerAttack != null)
            playerAttack.enabled = !locked;
    }

    // -----------------------------------------------------------
    // FADE UI CANVAS
    // -----------------------------------------------------------
    private void SetCanvasVisible(bool visible, bool instant = false)
    {
        if (canvasGroup == null) return;

        if (fadeUIRoutine != null) StopCoroutine(fadeUIRoutine);
        fadeUIRoutine = StartCoroutine(FadeCanvas(canvasGroup, visible, instant));
    }

    private IEnumerator FadeCanvas(CanvasGroup cg, bool visible, bool instant)
    {
        float target = visible ? 1f : 0f;
        float start = cg.alpha;
        float t = 0f;

        cg.interactable = visible;
        cg.blocksRaycasts = visible;

        if (instant)
        {
            cg.alpha = target;
            yield break;
        }

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }
    }

    // -----------------------------------------------------------
    // FADE BACKGROUND DIM
    // -----------------------------------------------------------
    private void SetDimVisible(bool visible, bool instant = false)
    {
        if (dimBackground == null) return;

        if (fadeDimRoutine != null) StopCoroutine(fadeDimRoutine);
        fadeDimRoutine = StartCoroutine(FadeDim(dimBackground, visible, instant));
    }

    private IEnumerator FadeDim(Image img, bool visible, bool instant)
    {
        float target = visible ? 0.75f : 0f;
        float start = img.color.a;
        float t = 0f;

        if (instant)
        {
            var c = img.color;
            c.a = target;
            img.color = c;
            yield break;
        }

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;

            var c = img.color;
            c.a = Mathf.Lerp(start, target, t / fadeDuration);
            img.color = c;

            yield return null;
        }
    }

    // -----------------------------------------------------------
    // RANDOM SYMBOL
    // -----------------------------------------------------------
    void OpenMinigame()
    {
        int count = Random.Range(1, 6);  // 1–5 symbols
        DrawSymbol[] arr = new DrawSymbol[count];

        for (int i = 0; i < count; i++)
            arr[i] = (DrawSymbol)Random.Range(0, System.Enum.GetValues(typeof(DrawSymbol)).Length);

        symbolDisplay.SetSymbols(arr);
    }


}
