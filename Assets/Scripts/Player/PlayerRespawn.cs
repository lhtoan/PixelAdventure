// using UnityEngine;

// public class PlayerRespawn : MonoBehaviour
// {
//     [SerializeField] private AudioClip checkpoint;
//     private Transform currentCheckpoint;
//     private Health playerHealth;

//     private int respawnCount = 0;          // đã hồi sinh bao nhiêu lần
//     [SerializeField] private int maxRespawn = 2;   // tối đa 2 lần, lần 3 game over

//     [SerializeField] private GameObject gameOverUI; // gán UI GameOver vào đây

//     private void Awake()
//     {
//         playerHealth = GetComponent<Health>();
//     }

//     public void RespawnCheck()
//     {
//         if (respawnCount >= maxRespawn)
//         {
//             TriggerGameOver();
//             return;
//         }

//         if (currentCheckpoint == null)
//             return;

//         respawnCount++;  // +1 lần respawn

//         playerHealth.Respawn();
//         transform.position = currentCheckpoint.position;

//         Debug.Log("Respawn: " + respawnCount + "/" + maxRespawn);
//     }

//     private void TriggerGameOver()
//     {
//         Time.timeScale = 0f;
//         gameOverUI.SetActive(true);

//         // Bật tất cả UI con bên trong GameOver
//         foreach (Transform child in gameOverUI.transform)
//             child.gameObject.SetActive(true);

//         Debug.Log("GAME OVER!");
//     }


//     private void OnTriggerEnter2D(Collider2D collision)
//     {
//         if (collision.CompareTag("CheckPoint"))
//         {
//             currentCheckpoint = collision.transform;
//             collision.GetComponent<Collider2D>().enabled = false;
//             collision.GetComponent<Animator>().SetTrigger("appear");
//             Debug.Log("checkpoint");
//         }
//     }

//     public void ResetRespawn()
//     {
//         respawnCount = 0;
//     }
// }


using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private AudioClip checkpoint;
    private Transform currentCheckpoint;
    private string currentCheckpointID;

    private Health playerHealth;

    private int respawnCount = 0;
    [SerializeField] private int maxRespawn = 2;

    [SerializeField] private GameObject gameOverUI;

    private void Awake()
    {
        playerHealth = GetComponent<Health>();
    }

    public void RespawnCheck()
    {
        if (respawnCount >= maxRespawn)
        {
            TriggerGameOver();
            return;
        }

        if (currentCheckpoint == null)
            return;

        respawnCount++;

        playerHealth.Respawn();
        transform.position = currentCheckpoint.position;

        Debug.Log("Respawn: " + respawnCount + "/" + maxRespawn);
    }

    private void TriggerGameOver()
    {
        Time.timeScale = 0f;
        gameOverUI.SetActive(true);

        foreach (Transform child in gameOverUI.transform)
            child.gameObject.SetActive(true);

        Debug.Log("GAME OVER!");
    }

    // private void OnTriggerEnter2D(Collider2D collision)
    // {
    //     if (collision.CompareTag("CheckPoint"))
    //     {
    //         CheckpointID cp = collision.GetComponent<CheckpointID>();
    //         if (cp != null)
    //         {
    //             currentCheckpointID = cp.checkpointID;
    //             Debug.Log("✅ Checkpoint reached: " + currentCheckpointID);
    //         }

    //         currentCheckpoint = collision.transform;

    //         collision.GetComponent<Collider2D>().enabled = false;
    //         collision.GetComponent<Animator>().SetTrigger("appear");

    //         Debug.Log("checkpoint");
    //     }
    // }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CheckPoint"))
        {
            CheckpointID cp = collision.GetComponent<CheckpointID>();
            if (cp != null)
            {
                currentCheckpointID = cp.checkpointID;
                Debug.Log("✅ Checkpoint reached: " + currentCheckpointID);
            }

            currentCheckpoint = collision.transform;

            collision.GetComponent<Collider2D>().enabled = false;
            collision.GetComponent<Animator>().SetTrigger("appear");

            Debug.Log("checkpoint");

            // ⭐ SAVE CHECKPOINT NGAY LẬP TỨC
            var saver = FindFirstObjectByType<SaveSystemController>();
            if (saver != null)
            {
                saver.SaveGame();     // Lưu file thật
            }

        }
    }



    public string GetCheckpointID()
    {
        return currentCheckpointID;
    }

    public void ResetRespawn()
    {
        respawnCount = 0;
    }

    // ================================
    // ⭐ HÀM THÊM CHO SAVE/LOAD
    // ================================

    public int RespawnCount => respawnCount;

    public void SetRespawnCount(int value)
    {
        respawnCount = value;
    }

    public void TeleportToCheckpoint(Transform t)
    {
        currentCheckpoint = t;
        transform.position = t.position;
    }
    public void TeleportToPosition(Vector2 pos)
    {
        transform.position = pos;
    }


}
