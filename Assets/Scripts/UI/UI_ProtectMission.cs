// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;

// public class UI_ProtectMission : MonoBehaviour
// {
//     [Header("Timer UI")]
//     public Image totalBar;
//     public Image currentBar;
//     public TMP_Text timerText;

//     [Header("NPC HP UI")]
//     public Health npcHealth;
//     public Image totalHp;
//     public Image currentHp;

//     [Header("Spawner Reference")]
//     public EnemySpawner spawner;

//     [Header("Trap Reference")]
//     public TrapManager trapManager;

//     private float maxTime;
//     private float currentTime;
//     private bool running = false;
//     private float maxHp;
//     private bool isMissionFailed = false;

//     // Không tự tắt UI trong Awake/Start — bạn tắt nó thủ công trong Hierarchy
//     private void Awake()
//     {
//         // intentionally empty to avoid race conditions
//     }

//     private void OnEnable()
//     {

//         if (npcHealth != null)
//             maxHp = npcHealth.GetStartingHealth();

//         if (currentHp != null)
//         {
//             currentHp.type = Image.Type.Filled;
//             currentHp.fillMethod = Image.FillMethod.Horizontal;
//             currentHp.fillOrigin = (int)Image.OriginHorizontal.Left;
//         }

//         if (totalHp != null)
//             totalHp.fillAmount = 1f;

//         if (npcHealth != null && currentHp != null)
//             currentHp.fillAmount = npcHealth.currentHealth / maxHp;
//     }

//     public void StartTimer(float duration)
//     {

//         // If parent (Canvas or UI root) is disabled, enable it first
//         if (transform.parent != null && !transform.parent.gameObject.activeInHierarchy)
//         {
//             transform.parent.gameObject.SetActive(true);
//         }

//         // bật UI (OnEnable sẽ chạy sau lệnh này)
//         gameObject.SetActive(true);
//         maxTime = duration;
//         currentTime = 0f;

//         if (totalBar != null) totalBar.fillAmount = 1f;
//         if (currentBar != null) currentBar.fillAmount = 0f;
//         if (timerText != null) timerText.text = FormatTime(maxTime);

//         running = true;
//         isMissionFailed = false;
//     }

//     private void Update()
//     {
//         if (running) UpdateTimer();

//         if (!isMissionFailed && npcHealth != null)
//             UpdateNpcHp();

//         if (!isMissionFailed && npcHealth != null && npcHealth.currentHealth <= 0)
//         {
//             isMissionFailed = true;
//             running = false;

//             if (spawner != null)
//             {
//                 spawner.StopSpawning();
//                 spawner.ClearAllEnemies();
//             }

//             if (trapManager != null)
//                 trapManager.StopTrapCycle();
//         }
//     }

//     private void UpdateTimer()
//     {
//         currentTime += Time.deltaTime;

//         if (currentTime > maxTime)
//         {
//             currentTime = maxTime;
//             running = false;
//         }

//         if (currentBar != null)
//             currentBar.fillAmount = currentTime / maxTime;

//         if (timerText != null)
//             timerText.text = FormatTime(maxTime - currentTime);
//     }

//     private void UpdateNpcHp()
//     {
//         if (npcHealth == null || currentHp == null) return;
//         float fill = npcHealth.currentHealth / maxHp;
//         currentHp.fillAmount = fill;
//     }

//     private string FormatTime(float time)
//     {
//         int minutes = Mathf.FloorToInt(time / 60f);
//         int seconds = Mathf.FloorToInt(time % 60f);
//         return $"{minutes:00}:{seconds:00}";
//     }
// }
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ProtectMission : MonoBehaviour
{
    [Header("Timer UI")]
    public Image totalBar;
    public Image currentBar;
    public TMP_Text timerText;

    [Header("NPC HP UI")]
    public Health npcHealth;
    public Image totalHp;
    public Image currentHp;

    [Header("Spawner Reference")]
    public EnemySpawner spawner;

    [Header("Trap Reference")]
    public TrapManager trapManager;

    private float maxTime;
    private float currentTime;
    private bool running = false;
    private float maxHp;
    private bool isMissionFailed = false;

    // 🔥 NEW — Thưởng coin khi bảo vệ NPC thành công
    [Header("Rewards")]
    public int minCoin = 80;
    public int maxCoin = 160;
    private GameManager gameManager;

    private void Awake()
    {
        // intentionally empty
    }

    private void OnEnable()
    {
        if (npcHealth != null)
            maxHp = npcHealth.GetStartingHealth();

        if (currentHp != null)
        {
            currentHp.type = Image.Type.Filled;
            currentHp.fillMethod = Image.FillMethod.Horizontal;
            currentHp.fillOrigin = (int)Image.OriginHorizontal.Left;
        }

        if (totalHp != null)
            totalHp.fillAmount = 1f;

        if (npcHealth != null && currentHp != null)
            currentHp.fillAmount = npcHealth.currentHealth / maxHp;

        // 🔥 NEW — Tìm GameManager
        gameManager = FindFirstObjectByType<GameManager>();
    }

    public void StartTimer(float duration)
    {
        if (transform.parent != null && !transform.parent.gameObject.activeInHierarchy)
        {
            transform.parent.gameObject.SetActive(true);
        }

        gameObject.SetActive(true);
        maxTime = duration;
        currentTime = 0f;

        if (totalBar != null) totalBar.fillAmount = 1f;
        if (currentBar != null) currentBar.fillAmount = 0f;
        if (timerText != null) timerText.text = FormatTime(maxTime);

        running = true;
        isMissionFailed = false;
    }

    private void Update()
    {
        if (running) UpdateTimer();

        if (!isMissionFailed && npcHealth != null)
            UpdateNpcHp();

        // ❌ NPC chết → thất bại
        if (!isMissionFailed && npcHealth != null && npcHealth.currentHealth <= 0)
        {
            isMissionFailed = true;
            running = false;

            if (spawner != null)
            {
                spawner.StopSpawning();
                spawner.ClearAllEnemies();
            }

            if (trapManager != null)
                trapManager.StopTrapCycle();

            return;
        }
    }

    private void UpdateTimer()
    {
        currentTime += Time.deltaTime;

        if (currentTime > maxTime)
        {
            currentTime = maxTime;
            running = false;

            // ⭐ NEW — Hết thời gian nhưng NPC vẫn sống = SUCCESS!
            if (!isMissionFailed)
            {
                MissionSuccess();
            }
        }

        if (currentBar != null)
            currentBar.fillAmount = currentTime / maxTime;

        if (timerText != null)
            timerText.text = FormatTime(maxTime - currentTime);
    }

    private void UpdateNpcHp()
    {
        if (npcHealth == null || currentHp == null) return;
        float fill = npcHealth.currentHealth / maxHp;
        currentHp.fillAmount = fill;
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:00}:{seconds:00}";
    }


    // ======================================================
    // ⭐⭐⭐ NEW: MISSION SUCCESS
    // ======================================================
    private void MissionSuccess()
    {
        Debug.Log("🎉 NPC protected successfully — Mission Completed!");

        // Dừng spawner & trap
        if (spawner != null)
            spawner.StopSpawning();

        if (trapManager != null)
            trapManager.StopTrapCycle();

        // 🎁 THƯỞNG COIN
        if (gameManager != null)
        {
            int reward = Random.Range(minCoin, maxCoin + 1);
            Debug.Log($"💰 You earned {reward} coins!");
            gameManager.AddScore(reward);
        }

        // Ẩn UI nếu muốn
        gameObject.SetActive(false);
    }
}
