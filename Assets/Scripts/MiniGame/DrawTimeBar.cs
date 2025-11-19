using UnityEngine;
using UnityEngine.UI;
using System;

public class DrawTimeBar : MonoBehaviour
{
    [Header("UI")]
    public Image totalTimeBar;      // thanh nền
    public Image currentTimeBar;    // thanh chạy

    [Header("Settings")]
    public float timeLimit = 3f;    // thời gian mỗi round

    private float timeRemaining;
    private bool isRunning = false;

    // Sự kiện callback khi hết giờ (để game xử lý)
    public Action OnTimeOut;

    private void Start()
    {
        if (totalTimeBar != null)
            totalTimeBar.fillAmount = 1f;

        currentTimeBar.fillAmount = 0f;
    }

    private void Update()
    {
        if (!isRunning) return;

        timeRemaining -= Time.unscaledDeltaTime;

        // Clamp
        if (timeRemaining < 0) timeRemaining = 0;

        // Update UI
        currentTimeBar.fillAmount = timeRemaining / timeLimit;

        // Hết giờ?
        if (timeRemaining <= 0)
        {
            isRunning = false;
            OnTimeOut?.Invoke();
        }
    }

    // Bắt đầu round mới
    public void StartTimer(float duration)
    {
        timeLimit = duration;
        timeRemaining = timeLimit;

        isRunning = true;

        if (totalTimeBar != null)
            totalTimeBar.fillAmount = 1f;

        if (currentTimeBar != null)
            currentTimeBar.fillAmount = 1f;
    }

    public void StopTimer()
    {
        isRunning = false;
    }
}
