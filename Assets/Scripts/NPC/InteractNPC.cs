using UnityEngine;

public class InteractNPC : MonoBehaviour
{
    [Header("References")]
    public EnemySpawner spawner;   // gắn SpawnPoint
    public Transform player;       // gắn Player
    public GameObject buttonF;     // gắn Button_F
    public TrapManager trapManager;
    public UI_ProtectMission timerUI;



    [Header("Settings")]
    public float interactRange = 2f;

    [Header("Button Offset (tự động theo hướng player)")]
    public Vector3 leftOffset = new Vector3(-1f, 0.5f, 0f);
    public Vector3 rightOffset = new Vector3(1f, 0.5f, 0f);

    private bool missionStarted = false;

    void Start()
    {
        if (buttonF != null)
            buttonF.SetActive(false);   // ẩn button ngay từ đầu
    }

    void Update()
    {
        if (missionStarted) return;
        if (player == null) return;

        float dist = Vector2.Distance(player.position, transform.position);

        // ⭐ Nếu player trong phạm vi tương tác
        if (dist <= interactRange)
        {
            if (buttonF != null)
            {
                buttonF.SetActive(true);

                // ⭐ Auto hướng bên Player
                if (player.position.x < transform.position.x)
                {
                    // Player bên trái NPC
                    buttonF.transform.position = transform.position + leftOffset;
                }
                else
                {
                    // Player bên phải NPC
                    buttonF.transform.position = transform.position + rightOffset;
                }
            }

            // Nhấn F để bắt đầu
            if (Input.GetKeyDown(KeyCode.F))
            {
                StartMission();
            }
        }
        else
        {
            // ⭐ Player rời xa → ẩn nút F
            if (buttonF != null)
                buttonF.SetActive(false);
        }
    }

    void StartMission()
    {
        missionStarted = true;

        // Ẩn button khi bắt đầu nhiệm vụ
        if (buttonF != null)
            buttonF.SetActive(false);

        if (timerUI != null)
            timerUI.StartTimer(spawner.totalSpawnTime);

        if (trapManager != null)
            trapManager.StartTrapCycle();

        // Bắt đầu spawn enemy
        spawner.StartSpawning();
        Debug.Log("Mission Started! Protect the NPC!");
    }
}
