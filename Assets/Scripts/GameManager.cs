// using UnityEngine;
// using TMPro;

// public class GameManager : MonoBehaviour
// {
//     // COIN
//     [SerializeField] private int score = 9999;
//     [SerializeField] private int skillPoints = 500;

//     [SerializeField] private TextMeshProUGUI scoreText;

//     // ⭐ SKILL POINT
//     [SerializeField] private TextMeshProUGUI skillPointText;

//     void Start()
//     {
//         UpdateScore();
//         UpdateSkillPoints();
//     }

//     // ==============================
//     //           SCORE
//     // ==============================
//     public void AddScore(int points)
//     {
//         score += points;
//         UpdateScore();
//     }

//     private void UpdateScore()
//     {
//         if (scoreText != null)
//             scoreText.text = score.ToString();
//     }

//     // ==============================
//     //        SKILL POINT
//     // ==============================
//     public void AddSkillPoint(int amount)
//     {
//         skillPoints += amount;

//         // ⭐ Luôn giữ skillPoints >= 0
//         if (skillPoints < 0)
//             skillPoints = 0;

//         UpdateSkillPoints();
//         Debug.Log("📘 Skill Point: " + skillPoints);
//     }


//     private void UpdateSkillPoints()
//     {
//         if (skillPointText != null)
//             skillPointText.text = skillPoints.ToString();
//     }

//     public int GetSkillPoints()
//     {
//         return skillPoints;
//     }

//     public int Score => score;
//     public int SkillPoints => skillPoints;

// }
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // COIN
    [SerializeField] private int score = 9999;
    [SerializeField] private int skillPoints = 500;

    [SerializeField] private TextMeshProUGUI scoreText;

    // ⭐ SKILL POINT
    [SerializeField] private TextMeshProUGUI skillPointText;

    void Start()
    {
        UpdateScore();
        UpdateSkillPoints();
    }

    // ==============================
    // SCORE
    // ==============================
    public void AddScore(int points)
    {
        score += points;
        UpdateScore();
    }

    private void UpdateScore()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    // ==============================
    // SKILL POINT
    // ==============================
    public void AddSkillPoint(int amount)
    {
        skillPoints += amount;

        if (skillPoints < 0)
            skillPoints = 0;

        UpdateSkillPoints();
        Debug.Log("📘 Skill Point: " + skillPoints);
    }

    private void UpdateSkillPoints()
    {
        if (skillPointText != null)
            skillPointText.text = skillPoints.ToString();
    }

    public int GetSkillPoints()
    {
        return skillPoints;
    }

    public int Score => score;
    public int SkillPoints => skillPoints;

    // ==========================================
    // ⭐ HÀM DÙNG CHO SAVE / LOAD
    // ==========================================
    public void ForceSetScore(int value)
    {
        score = value;
        UpdateScore();
    }

    public void ForceSetSkillPoints(int value)
    {
        skillPoints = value;
        UpdateSkillPoints();
    }
}
