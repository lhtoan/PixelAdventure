using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Menu Root")]
    [SerializeField] private GameObject menuRoot;

    [Header("Sub Menus")]
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject pauseMenu;

    [Header("Player")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerAttack playerAttack;

    // private void Start()
    // {
    //     ShowStartMenu();

    //     if (playerController) playerController.enabled = false;
    //     if (playerAttack) playerAttack.inputLocked = true;

    //     Time.timeScale = 0f;
    // }
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
        StartGame();
        Debug.Log("START GAME");
    }

    public void OnQuitClicked()
    {
        Debug.Log("QUIT GAME");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
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
        Debug.Log("RETURN TO MAIN MENU");

        Time.timeScale = 0f;

        menuRoot.SetActive(true);
        startMenu.SetActive(true);
        pauseMenu.SetActive(false);

        if (playerController) playerController.enabled = false;
        if (playerAttack) playerAttack.inputLocked = true;
    }

    // public void OnPlayAgainClicked()
    // {
    //     Debug.Log("PLAY AGAIN");

    //     Time.timeScale = 1f;

    //     // Load lại scene -> game mới hoàn toàn
    //     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    // }
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


    // =============================
    // MENU CONTROL
    // =============================

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

    // public void OnReloadClicked()
    // {
    //     Debug.Log("RELOAD GAME FROM SAVE");

    //     // 1) Load dữ liệu
    //     FindFirstObjectByType<SaveSystemController>().LoadGame();

    //     // 2) Thoát pause menu, tiếp tục chơi
    //     ResumeGame();
    // }

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
        SaveData data = saver.BuildSaveData();

        data.isManualSave = true;

        Vector3 pos = saver.GetPlayerPosition();
        data.posX = pos.x;
        data.posY = pos.y;

        SaveManager.Save(data);

        Debug.Log($"💾 MANUAL SAVE at position ({data.posX}, {data.posY})");
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
