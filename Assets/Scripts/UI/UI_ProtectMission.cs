using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ProtectMission : MonoBehaviour
{
    [Header("Timer UI")]
    public Image totalBar;        // Timer_Total
    public Image currentBar;      // Timer_Current
    public TMP_Text timerText;    // Timer_Text

    [Header("NPC HP UI")]
    public Health npcHealth;          // gắn script Health của NPC
    public Image totalHp;             // TotalHp image
    public Image currentHp;           // CurrentHp image

    [Header("Spawner Reference")]
    public EnemySpawner spawner;

    [Header("Trap Reference")]
    public TrapManager trapManager;



    private float maxTime;
    private float currentTime;
    private bool running = false;

    private float maxHp;

    private bool isMissionFailed = false;

    private void Start()
    {
        gameObject.SetActive(false); // ẩn UI khi bắt đầu

        // chuẩn bị HP NPC nếu được gắn
        if (npcHealth != null)
            maxHp = npcHealth.GetStartingHealth();

        // đảm bảo currentHp fill từ trái sang phải
        if (currentHp != null)
        {
            currentHp.type = Image.Type.Filled;
            currentHp.fillMethod = Image.FillMethod.Horizontal;
            currentHp.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
    }

    public void StartTimer(float duration)
    {
        maxTime = duration;
        currentTime = 0f;

        totalBar.fillAmount = 1f;
        currentBar.fillAmount = 0f;
        timerText.text = FormatTime(maxTime);

        gameObject.SetActive(true);
        running = true;

        // khởi động HP UI
        if (npcHealth != null)
        {
            maxHp = npcHealth.GetStartingHealth();
            currentHp.fillAmount = npcHealth.currentHealth / maxHp;
            totalHp.fillAmount = 1f;
        }

        isMissionFailed = false;
    }

    private void Update()
    {
        if (running)
            UpdateTimer();

        if (!isMissionFailed && npcHealth != null)
            UpdateNpcHp();

        // ⭐ NPC chết → thua
        if (!isMissionFailed && npcHealth != null && npcHealth.currentHealth <= 0)
        {
            isMissionFailed = true;
            running = false;
            Debug.Log("Thua rồi! NPC đã chết!");

            if (spawner != null)
            {
                spawner.StopSpawning();
                spawner.ClearAllEnemies();  // ⭐ XÓA enemy ngay lập tức!
            }

            if (trapManager != null)
                trapManager.StopTrapCycle();
        }
    }

    private void UpdateTimer()
    {
        currentTime += Time.deltaTime;

        if (currentTime > maxTime)
        {
            currentTime = maxTime;
            running = false;
        }

        currentBar.fillAmount = currentTime / maxTime;

        float remaining = maxTime - currentTime;
        timerText.text = FormatTime(remaining);
    }

    private void UpdateNpcHp()
    {
        float fill = npcHealth.currentHealth / maxHp;
        currentHp.fillAmount = fill;
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        return $"{minutes:00}:{seconds:00}";
    }
}
