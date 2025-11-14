using UnityEngine;

public class InteractNPC : MonoBehaviour
{
    public EnemySpawner spawner;       // gán vào spawnpoint
    public float interactRange = 2f;   // khoảng cách để bấm E
    public Transform player;

    private bool missionStarted = false;

    void Update()
    {
        if (missionStarted) return;

        if (player == null) return;

        float dist = Vector2.Distance(player.position, transform.position);

        if (dist <= interactRange && Input.GetKeyDown(KeyCode.F))
        {
            StartMission();
        }
    }

    void StartMission()
    {
        missionStarted = true;
        spawner.StartSpawning();
        Debug.Log("Mission Started! Protect the NPC!");
    }
}
