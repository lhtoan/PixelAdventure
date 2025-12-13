using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Menu Root")]
    [SerializeField] private GameObject menuRoot;

    [Header("Sub Menus")]
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject confirmBackPanel;
    [SerializeField] private GameObject confirmQuitPanel;
    [SerializeField] private GameObject againPanel;
    [SerializeField] private GameObject bossWinPanel;




    [Header("Player")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerAttack playerAttack;

    private void Awake()
    {
        // Nếu là lần đầu vào game → khóa player ngay lập tức
        if (PlayerPrefs.GetInt("IsReloadEvent", 0) == 0)
        {
            if (playerController) playerController.enabled = false;
            if (playerAttack) playerAttack.inputLocked = true;
        }
    }


    private void Start()
    {
        if (PlayerPrefs.GetInt("IsReloadEvent", 0) == 1)
        {
            PlayerPrefs.SetInt("IsReloadEvent", 0);

            // ⭐ KHÔNG tắt menuRoot — UI cần active để pause hoạt động
            menuRoot.SetActive(true);
            startMenu.SetActive(false);  // Không hiện menu start khi reload
            pauseMenu.SetActive(false);

            if (playerController) playerController.enabled = true;
            if (playerAttack) playerAttack.inputLocked = false;

            Time.timeScale = 1f;
            return;
        }

        // ⭐ Trường hợp mở game lần đầu
        ShowStartMenu();

        if (playerController) playerController.enabled = false;
        if (playerAttack) playerAttack.inputLocked = true;

        Time.timeScale = 0f;
    }




    // =============================
    // BUTTON EVENTS
    // =============================

    public void OnPlayClicked()
    {
        Debug.Log("START NEW GAME");

        // Đánh dấu: KHÔNG load save
        PlayerPrefs.SetInt("ShouldLoadSave", 0);
        PlayerPrefs.SetInt("IsReloadEvent", 1);

        Time.timeScale = 1f;

        // Load lại scene → world reset, player reset
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    // public void OnQuitClicked()
    // {
    //     Debug.Log("QUIT GAME");
    //     Application.Quit();

    // #if UNITY_EDITOR
    //         UnityEditor.EditorApplication.isPlaying = false;
    // #endif
    // }

    public void OnQuitClicked()
    {
        Debug.Log("SHOW CONFIRM QUIT POPUP");

        confirmQuitPanel.SetActive(true);

        // Vì đang ở menu, không cần thay đổi timeScale
    }

    public void OnConfirmQuitYes()
    {
        Debug.Log("QUIT GAME CONFIRMED");
        Application.Quit();

    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }

    public void OnConfirmQuitNo()
    {
        Debug.Log("CANCEL QUIT");
        confirmQuitPanel.SetActive(false);
    }




    public void OnResumeClicked()
    {
        ResumeGame();
    }

    public void OnRestartClicked()
    {
        Debug.Log("RESTART GAME");

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnBackToMenuClicked()
    {
        Debug.Log("BACK → SHOW CONFIRM EXIT POPUP");

        // Hiện panel Confirm Exit
        confirmBackPanel.SetActive(true);

        // Tạm dừng game (vì đang ở pause)
        Time.timeScale = 0f;

        // Khóa input tấn công
        if (playerAttack) playerAttack.inputLocked = true;
    }

    public void OnConfirmExitYes()
    {
        Debug.Log("CONFIRMED: RETURN TO MAIN MENU + AUTO SAVE");

        var saver = FindFirstObjectByType<SaveSystemController>();
        saver.ManualSave();   // ⭐ auto save trước khi thoát

        // Trở về Menu chính
        menuRoot.SetActive(true);
        startMenu.SetActive(true);
        pauseMenu.SetActive(false);
        confirmBackPanel.SetActive(false);
        againPanel.SetActive(false);

        Time.timeScale = 0f;

        if (playerController) playerController.enabled = false;
        if (playerAttack) playerAttack.inputLocked = true;
    }

    // BACK ở UI AGAIN (Game Over)
    public void OnAgainBackClicked()
    {
        Debug.Log("AGAIN → BACK TO MENU");

        // Tắt panel again
        againPanel.SetActive(false);

        // Quay về Start Menu
        menuRoot.SetActive(true);
        startMenu.SetActive(true);
        pauseMenu.SetActive(false);
        confirmBackPanel.SetActive(false);

        // Dừng thời gian
        Time.timeScale = 0f;

        // Khóa player
        if (playerController) playerController.enabled = false;
        if (playerAttack) playerAttack.inputLocked = true;
    }


    public void OnConfirmExitNo()
    {
        Debug.Log("CANCEL EXIT");

        confirmBackPanel.SetActive(false);

        // Quay lại Pause Menu
        pauseMenu.SetActive(true);
    }


    public void OnPlayAgainClicked()
    {
        Debug.Log("PLAY AGAIN (OPTION A)");

        // Đánh dấu cần load save
        PlayerPrefs.SetInt("ShouldLoadSave", 1);

        // Đánh dấu reload → không hiện menu Start
        PlayerPrefs.SetInt("IsReloadEvent", 1);

        Time.timeScale = 1f;

        // Reload scene → reset enemy, items, world
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void StartGame()
    {
        Debug.Log("StartGame(): Hiding Start + Pause");

        startMenu.SetActive(false);
        pauseMenu.SetActive(false);

        if (playerController) playerController.enabled = true;
        if (playerAttack) playerAttack.inputLocked = false;

        Time.timeScale = 1f;
    }

    public void ShowStartMenu()
    {
        Debug.Log("ShowStartMenu()");
        menuRoot.SetActive(true);
        startMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void ShowPauseMenu()
    {
        Debug.Log("ShowPauseMenu(): PAUSE OPENED");

        menuRoot.SetActive(true);
        pauseMenu.SetActive(true);
        startMenu.SetActive(false);

        Time.timeScale = 0f;

        if (playerAttack) playerAttack.inputLocked = true;
    }

    public void ResumeGame()
    {
        Debug.Log("ResumeGame(): PAUSE CLOSED");

        // KHÔNG tắt menuRoot – chỉ tắt pauseMenu
        pauseMenu.SetActive(false);

        // Giữ nguyên startMenu tắt khi đang chơi
        startMenu.SetActive(false);

        Time.timeScale = 1f;

        if (playerAttack) playerAttack.inputLocked = false;
    }

    public void OnReloadClicked()
    {
        Debug.Log("RELOAD GAME (OPTION A)");

        PlayerPrefs.SetInt("ShouldLoadSave", 1);       // ✔ load save
        PlayerPrefs.SetInt("IsReloadEvent", 1);        // ✔ đánh dấu reload (không hiện start menu)

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnManualSaveClicked()
    {
        var saver = FindFirstObjectByType<SaveSystemController>();
        saver.ManualSave();

    }

    public void OnContinueClicked()
    {
        Debug.Log("CONTINUE GAME");

        // ❌ Không có file save → chơi game mới luôn
        if (!SaveManager.HasSave())
        {
            Debug.Log("⚠ No save found → Starting NEW GAME");
            StartGame();      // chạy như nút Play
            return;
        }

        // ✔ Có file save → Continue
        PlayerPrefs.SetInt("ShouldLoadSave", 1);
        PlayerPrefs.SetInt("IsReloadEvent", 1);

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void ShowBossWin()
    {
        StartCoroutine(ShowBossWinAfterDelay());

    }

    private IEnumerator ShowBossWinAfterDelay()
    {
        // Chờ camera transition xong
        yield return new WaitForSecondsRealtime(2f);

        // ⭐ CHỈ BẬT UI TẠI ĐÂY — CAMERA ĐÃ ỔN ĐỊNH ⭐
        bossWinPanel.SetActive(true);
        menuRoot.SetActive(true);

        Time.timeScale = 0f;

        if (playerController) playerController.enabled = false;
        if (playerAttack) playerAttack.inputLocked = true;
    }

    public void ResumeGameWin()
    {
        bossWinPanel.SetActive(false);

        startMenu.SetActive(false);

        Time.timeScale = 1f;

        if (playerController) playerController.enabled = true;   // ⭐ MUST
        if (playerAttack) playerAttack.inputLocked = false;      // attack unlock
    }


    public void OnBossWinBack()
    {
        bossWinPanel.SetActive(false);

        menuRoot.SetActive(true);
        startMenu.SetActive(true);

        Time.timeScale = 0f;

        if (playerController) playerController.enabled = false;
        if (playerAttack) playerAttack.inputLocked = true;
    }




    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("TAB PRESSED | pauseMenu active = " + pauseMenu.activeSelf);

            if (!pauseMenu.activeSelf)
            {
                Debug.Log("→ Opening Pause Menu");
                ShowPauseMenu();
            }
            else
            {
                Debug.Log("→ Closing Pause Menu");
                ResumeGame();
            }
        }
    }
}
