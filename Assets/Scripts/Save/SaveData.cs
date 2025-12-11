using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public string checkpointID;

    public float health;
    public int score;
    public int skillPoints;

    public List<string> unlockedSkills = new List<string>();

    public int respawnCount;

    // ⭐ thêm để lưu vị trí manual save
    public bool isManualSave;
    public float posX;
    public float posY;
}
