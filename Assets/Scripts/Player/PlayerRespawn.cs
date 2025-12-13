using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Transform currentCheckpoint;
    private string currentCheckpointID;

    private Health playerHealth;

    private int respawnCount = 0;
    [SerializeField] private int maxRespawn = 2;

    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private AudioManager audioManager;


    private void Awake()
    {
        playerHealth = GetComponent<Health>();

        if (audioManager == null)
        {
            GameObject gm = GameObject.FindGameObjectWithTag("GameManager");
            if (gm != null)
                audioManager = gm.GetComponent<AudioManager>();
        }
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

        if (audioManager != null && audioManager.gameoverClip != null)
            audioManager.PlaySFX(audioManager.gameoverClip, 0.5f);

        // ⭐ (optional) stop BGM để tiếng game over rõ hơn
        if (audioManager != null)
            audioManager.StopBGM();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CheckPoint"))
        {
            CheckpointID cp = collision.GetComponent<CheckpointID>();
            if (cp != null)
            {
                currentCheckpointID = cp.checkpointID;
            }

            currentCheckpoint = collision.transform;

            collision.GetComponent<Collider2D>().enabled = false;
            collision.GetComponent<Animator>().SetTrigger("appear");

            if (PlayerPrefs.GetInt("IsReloadEvent", 0) == 0)
            {
                var saver = FindFirstObjectByType<SaveSystemController>();
                if (saver != null)
                    saver.SaveAtCheckpoint();
            }


            if (audioManager != null && audioManager.checkpointClip != null)
                audioManager.PlaySFX(audioManager.checkpointClip, 1);



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

    public void SetLoadedCheckpoint(string id)
    {
        currentCheckpointID = id;
        // tìm checkpoint Transform cho chắc (optional)
    }

    

}
