using UnityEngine;

public class SaveSystemController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health playerHealth; // ⭐ KÉO PLAYER VÀO ĐÂY

    private PlayerRespawn playerRespawn;
    private GameManager gameManager;
    private PlayerSkill playerSkill;

    private void Awake()
    {
        playerRespawn = FindFirstObjectByType<PlayerRespawn>();
        gameManager = FindFirstObjectByType<GameManager>();
        playerSkill = FindFirstObjectByType<PlayerSkill>();

    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("ShouldLoadSave", 0) == 1)
        {
            PlayerPrefs.SetInt("ShouldLoadSave", 0);  // reset flag
            LoadGame();  // load dữ liệu player
        }
    }


    // ===============================================
    // BUILD SAVE DATA
    // ===============================================
    public SaveData BuildSaveData()
    {
        SaveData data = new SaveData();

        data.checkpointID = playerRespawn.GetCheckpointID();
        data.respawnCount = 0;

        data.health = playerHealth.currentHealth;
        data.score = gameManager.Score;
        data.skillPoints = gameManager.SkillPoints;

        data.unlockedSkills = playerSkill.GetUnlockedSkillsList();

        return data;
    }

    // ===============================================
    // SAVE
    // ===============================================
    // public void SaveGame()
    // {
    //     SaveData data = BuildSaveData();
    //     SaveManager.Save(data);
    // }


    // ===============================================
    // LOAD
    // ===============================================
    public void LoadGame()
    {
        SaveData data = SaveManager.Load();
        if (data == null) return;

        // Load máu
        playerHealth.SetHealth(data.health);

        // Load coin & skill point
        gameManager.ForceSetScore(data.score);
        gameManager.ForceSetSkillPoints(data.skillPoints);

        // Load skill
        foreach (string s in data.unlockedSkills)
        {
            PlayerSkill.SkillType type =
                (PlayerSkill.SkillType)System.Enum.Parse(typeof(PlayerSkill.SkillType), s);
            playerSkill.UnlockSkill(type);
        }

        // Load respawn
        playerRespawn.SetRespawnCount(0);

        // ⭐ Load vị trí
        // ⭐ Load vị trí nếu manual save
        if (data.isManualSave)
        {
            Vector2 pos = new Vector2(data.posX, data.posY);
            playerRespawn.TeleportToPosition(pos);   // ✔ DI CHUYỂN PLAYER
        }
        else
        {
            MovePlayerToCheckpoint(data.checkpointID); // checkpoint load
        }

    }

    public Vector3 GetPlayerPosition()
    {
        return playerHealth.transform.position;
    }


    private void MovePlayerToCheckpoint(string cpID)
    {
        if (string.IsNullOrEmpty(cpID)) return;

        var checkpoints = FindObjectsByType<CheckpointID>(FindObjectsSortMode.None);

        foreach (var cp in checkpoints)
        {
            if (cp.checkpointID == cpID)
            {
                playerRespawn.TeleportToCheckpoint(cp.transform);
                playerRespawn.SetLoadedCheckpoint(cp.checkpointID);   // ⭐ FIX RẤT QUAN TRỌNG

                break;
            }
        }
    }

    public void SaveAtCheckpoint()
    {
        SaveData data = BuildSaveData();

        data.isManualSave = false;   // ✅ RẤT QUAN TRỌNG
        data.posX = 0;
        data.posY = 0;

        SaveManager.Save(data);
    }

    public void ManualSave()
    {
        SaveData data = BuildSaveData();

        data.isManualSave = true;    // ✅ PHÂN BIỆT RÕ
        Vector3 pos = GetPlayerPosition();
        data.posX = pos.x;
        data.posY = pos.y;

        SaveManager.Save(data);
    }



    public void DebugSaveData(SaveData data)
    {
        Debug.Log("===== 📦 SAVE DATA =====");
        Debug.Log("CheckpointID: " + data.checkpointID);
        Debug.Log("Health: " + data.health);
        Debug.Log("Score: " + data.score);
        Debug.Log("SkillPoints: " + data.skillPoints);
        Debug.Log("RespawnCount: " + data.respawnCount);

        Debug.Log("Unlocked Skills:");
        foreach (var s in data.unlockedSkills)
            Debug.Log(" - " + s);

        Debug.Log("=========================");
    }
}
