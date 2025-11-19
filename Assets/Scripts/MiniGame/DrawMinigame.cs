// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections;

// public class DrawMinigame : MonoBehaviour
// {
//     [Header("UI References")]
//     public CanvasGroup canvasGroup;
//     public Image dimBackground;
//     public float fadeDuration = 0.25f;
//     public bool pauseGameWhenOpen = true;

//     [Header("Player Control")]
//     private PlayerController playerController;
//     private PlayerAttack playerAttack;

//     private bool isOpen = false;
//     private Coroutine fadeUIRoutine;
//     private Coroutine fadeDimRoutine;

//     [Header("Symbol")]
//     public DrawSymbol currentSymbol;
//     public SymbolDisplay symbolDisplay;

//     [Header("Draw System")]
//     public GameObject drawArea;
//     public DrawLine drawLineScript;
//     public DrawLine drawLine;



//     private void Awake()
//     {
//         GameObject playerObj = GameObject.FindWithTag("Player");
//         if (playerObj != null)
//         {
//             playerController = playerObj.GetComponent<PlayerController>();
//             playerAttack = playerObj.GetComponent<PlayerAttack>();
//         }
//     }

//     private void Start()
//     {
//         // Ẩn UI hoàn toàn lúc đầu
//         SetCanvasVisible(false, true);
//         SetDimVisible(false, true);

//         // Tắt SymbolTarget lúc đầu
//         if (symbolDisplay != null)
//             symbolDisplay.gameObject.SetActive(false);
//     }

//     private void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.L))
//             ToggleMinigame();

//         if (Input.GetKeyDown(KeyCode.Escape) && isOpen)
//             CloseMinigame();
//     }

//     // -----------------------------------------------------------
//     // TOGGLE Minigame
//     // -----------------------------------------------------------
//     private void ToggleMinigame()
//     {
//         isOpen = !isOpen;

//         SetCanvasVisible(isOpen);
//         SetDimVisible(isOpen);
//         LockPlayerControls(isOpen);

//         if (pauseGameWhenOpen)
//             Time.timeScale = isOpen ? 0f : 1f;

//         if (isOpen)
//         {
//             symbolDisplay.gameObject.SetActive(true);

//             drawArea.SetActive(true);
//             drawLineScript.enabled = true;

//             OpenMinigame();
//         }
//         else
//         {
//             symbolDisplay.gameObject.SetActive(false);

//             drawArea.SetActive(false);
//             drawLineScript.enabled = false;

//             if (drawLineScript != null)
//                 drawLineScript.ResetLine();
//         }
//     }


//     // -----------------------------------------------------------
//     // CLOSE Minigame
//     // -----------------------------------------------------------
//     private void CloseMinigame()
//     {
//         isOpen = false;

//         SetCanvasVisible(false);
//         SetDimVisible(false);
//         LockPlayerControls(false);

//         if (pauseGameWhenOpen)
//             Time.timeScale = 1f;

//         symbolDisplay.gameObject.SetActive(false);

//         if (drawLine != null)
//         {
//             drawLine.ResetLine();
//         }
//     }


//     // -----------------------------------------------------------
//     // LOCK PLAYER INPUT
//     // -----------------------------------------------------------
//     private void LockPlayerControls(bool locked)
//     {
//         if (playerController != null)
//             playerController.enabled = !locked;

//         if (playerAttack != null)
//             playerAttack.enabled = !locked;
//     }

//     // -----------------------------------------------------------
//     // FADE UI CANVAS
//     // -----------------------------------------------------------
//     private void SetCanvasVisible(bool visible, bool instant = false)
//     {
//         if (canvasGroup == null) return;

//         if (fadeUIRoutine != null) StopCoroutine(fadeUIRoutine);
//         fadeUIRoutine = StartCoroutine(FadeCanvas(canvasGroup, visible, instant));
//     }

//     private IEnumerator FadeCanvas(CanvasGroup cg, bool visible, bool instant)
//     {
//         float target = visible ? 1f : 0f;
//         float start = cg.alpha;
//         float t = 0f;

//         cg.interactable = visible;
//         cg.blocksRaycasts = visible;

//         if (instant)
//         {
//             cg.alpha = target;
//             yield break;
//         }

//         while (t < fadeDuration)
//         {
//             t += Time.unscaledDeltaTime;
//             cg.alpha = Mathf.Lerp(start, target, t / fadeDuration);
//             yield return null;
//         }
//     }

//     // -----------------------------------------------------------
//     // FADE BACKGROUND DIM
//     // -----------------------------------------------------------
//     private void SetDimVisible(bool visible, bool instant = false)
//     {
//         if (dimBackground == null) return;

//         if (fadeDimRoutine != null) StopCoroutine(fadeDimRoutine);
//         fadeDimRoutine = StartCoroutine(FadeDim(dimBackground, visible, instant));
//     }

//     private IEnumerator FadeDim(Image img, bool visible, bool instant)
//     {
//         float target = visible ? 0.75f : 0f;
//         float start = img.color.a;
//         float t = 0f;

//         if (instant)
//         {
//             var c = img.color;
//             c.a = target;
//             img.color = c;
//             yield break;
//         }

//         while (t < fadeDuration)
//         {
//             t += Time.unscaledDeltaTime;

//             var c = img.color;
//             c.a = Mathf.Lerp(start, target, t / fadeDuration);
//             img.color = c;

//             yield return null;
//         }
//     }

//     // -----------------------------------------------------------
//     // RANDOM SYMBOL
//     // -----------------------------------------------------------
//     void OpenMinigame()
//     {
//         int count = Random.Range(1, 6);  // 1–5 symbols
//         DrawSymbol[] arr = new DrawSymbol[count];

//         for (int i = 0; i < count; i++)
//             arr[i] = (DrawSymbol)Random.Range(0, System.Enum.GetValues(typeof(DrawSymbol)).Length);

//         symbolDisplay.SetSymbols(arr);
//     }


// }

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DrawMinigame : MonoBehaviour
{
    // ============================================================
    // 1. --- INSPECTOR FIELDS ---
    // ============================================================
    [Header("UI References")]
    public CanvasGroup canvasGroup;
    public Image dimBackground;
    public float fadeDuration = 0.25f;

    [Header("Pause Settings")]
    public bool pauseGameWhenOpen = true;

    [Header("Symbol Display")]
    public SymbolDisplay symbolDisplay;

    [Header("Draw System")]
    public GameObject drawArea;
    public DrawLine drawLineScript;

    [Header("Recording")]
    public bool enableRecording = false;
    public GestureRecorder recorder;

    [Header("Templates (Matching)")]
    public List<GestureTemplateSO> templates;


    // ============================================================
    // 2. --- INTERNAL STATE ---
    // ============================================================
    private bool isOpen = false;
    private Coroutine fadeUIRoutine;
    private Coroutine fadeDimRoutine;

    // Player
    private PlayerController playerController;
    private PlayerAttack playerAttack;

    // Sequence
    private DrawSymbol[] currentSequence;
    private int currentIndex = 0;


    // ============================================================
    // 3. --- UNITY LIFECYCLE ---
    // ============================================================
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
        SetCanvasVisible(false, true);
        SetDimVisible(false, true);

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


    // ============================================================
    // 4. --- OPEN / CLOSE MINIGAME ---
    // ============================================================
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
            OpenUI();
            OpenMinigameLogic();
        }
        else
        {
            CloseUI();
        }
    }

    private void CloseMinigame()
    {
        isOpen = false;

        SetCanvasVisible(false);
        SetDimVisible(false);

        if (pauseGameWhenOpen)
            Time.timeScale = 1f;

        LockPlayerControls(false);

        if (symbolDisplay != null)
            symbolDisplay.gameObject.SetActive(false);

        if (drawLineScript != null)
            drawLineScript.ResetLine();
    }


    // ============================================================
    // 5. --- UI SHOW / HIDE ---
    // ============================================================
    private void OpenUI()
    {
        symbolDisplay.gameObject.SetActive(true);
        drawArea.SetActive(true);
        drawLineScript.enabled = true;
    }

    private void CloseUI()
    {
        symbolDisplay.gameObject.SetActive(false);
        drawArea.SetActive(false);
        drawLineScript.enabled = false;

        if (drawLineScript != null)
            drawLineScript.ResetLine();
    }


    // ============================================================
    // 6. --- RECORD & MATCH HANDLING ---
    // ============================================================
    private void OpenMinigameLogic()
    {
        GenerateRandomSequence();

        if (enableRecording && recorder != null)
            recorder.BeginRecording();
    }

    private void HandleRecording(List<Vector2> normalizedPts)
    {
        if (recorder != null && recorder.isRecording)
            recorder.Capture(normalizedPts);
    }


    // ------ PLAY MODE: MATCHING ------
    private void HandleMatching(List<Vector2> normalizedPts)
    {
        DrawSymbol result = GestureRecognizer.Recognize(normalizedPts, templates);
        Debug.Log(">>> MATCH RESULT = " + result);

        if (currentSequence == null || currentSequence.Length == 0)
        {
            Debug.LogWarning("No sequence active!");
            return;
        }

        DrawSymbol expected = currentSequence[currentIndex];

        // ----- CORRECT -----
        if (result == expected)
        {
            Debug.Log($"CORRECT ({currentIndex + 1}/{currentSequence.Length})");

            currentIndex++;

            // Hoàn thành toàn bộ chuỗi
            if (currentIndex >= currentSequence.Length)
            {
                Debug.Log("🎉 Bảo vệ thành công!");
                OnSequenceCompleted();
            }
        }
        // ----- WRONG -----
        else
        {
            Debug.Log($"❌ SAI! Expected {expected} but got {result}");
            OnWrongSymbol();
        }

        drawLineScript.ResetLine();
    }


    // ============================================================
    // 7. --- ENTRY FROM DrawLine ---
    // ============================================================
    public void OnPlayerDrawFinished(List<Vector2> normalizedPts)
    {
        Debug.Log($"[DrawMinigame] Player Draw = {(normalizedPts?.Count ?? 0)} pts");

        if (enableRecording)
        {
            HandleRecording(normalizedPts);
            return;
        }

        HandleMatching(normalizedPts);
    }


    // ============================================================
    // 8. --- RANDOM SEQUENCE GENERATION ---
    // ============================================================
    private void GenerateRandomSequence()
    {
        int count = Random.Range(1, 6);

        currentSequence = new DrawSymbol[count];
        currentIndex = 0;

        for (int i = 0; i < count; i++)
        {
            currentSequence[i] = (DrawSymbol)Random.Range(
                0, System.Enum.GetValues(typeof(DrawSymbol)).Length
            );
        }

        symbolDisplay.SetSymbols(currentSequence);
    }


    // ============================================================
    // 9. --- CORRECT / WRONG HANDLING ---
    // ============================================================
    private void OnSequenceCompleted()
    {
        // Ở đây bạn muốn làm gì thì làm
        // Hiện tại chỉ log
        Debug.Log("⚡ Chuỗi ký hiệu đã hoàn thành! Skill kích hoạt!");
    }

    private void OnWrongSymbol()
    {
        // Reset lại từ đầu để vẽ lại toàn bộ chuỗi
        // currentIndex = 0;
    }


    // ============================================================
    // 10. --- PLAYER LOCK ---
    // ============================================================
    private void LockPlayerControls(bool locked)
    {
        if (playerController != null)
            playerController.enabled = !locked;

        if (playerAttack != null)
            playerAttack.enabled = !locked;
    }


    // ============================================================
    // 11. --- FADE UI ---
    // ============================================================
    private void SetCanvasVisible(bool visible, bool instant = false)
    {
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

    private void SetDimVisible(bool visible, bool instant = false)
    {
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
            Color c = img.color;
            c.a = target;
            img.color = c;
            yield break;
        }

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;

            Color c = img.color;
            c.a = Mathf.Lerp(start, target, t / fadeDuration);
            img.color = c;

            yield return null;
        }
    }
}
