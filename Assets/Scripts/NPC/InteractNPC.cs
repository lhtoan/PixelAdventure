using UnityEngine;

public class InteractNPC : MonoBehaviour
{
    [Header("References")]
    public EnemySpawner spawner;
    public Transform player;
    public GameObject buttonF;
    // public TrapManager trapManager;
    public UI_ProtectMission timerUI;
    public CameraMissionController cameraControl;   // ⭐ NEW

    [Header("Settings")]
    public float interactRange = 2f;

    [Header("Button Offset")]
    public Vector3 leftOffset = new Vector3(-1f, 0.5f, 0f);
    public Vector3 rightOffset = new Vector3(1f, 0.5f, 0f);

    private bool missionStarted = false;

    void Start()
    {
        if (buttonF != null)
            buttonF.SetActive(false);
    }

    void Update()
    {
        if (missionStarted) return;
        if (player == null) return;

        float dist = Vector2.Distance(player.position, transform.position);

        if (dist <= interactRange)
        {
            if (buttonF != null)
            {
                buttonF.SetActive(true);
                buttonF.transform.position =
                    player.position.x < transform.position.x ?
                    transform.position + leftOffset :
                    transform.position + rightOffset;
            }

            if (Input.GetKeyDown(KeyCode.F))
                StartMission();
        }
        else
        {
            if (buttonF != null)
                buttonF.SetActive(false);
        }
    }

    void StartMission()
    {
        missionStarted = true;

        if (buttonF != null)
            buttonF.SetActive(false);

        if (timerUI != null)
            timerUI.StartTimer(spawner.totalSpawnTime);

        // if (trapManager != null)
        //     trapManager.StartTrapCycle();

        spawner.StartSpawning();
        Debug.Log("Mission Started!");

        // ⭐ Camera zoom-out + pixel perfect change
        if (cameraControl != null)
            cameraControl.ApplyMissionCamera();
    }
}
