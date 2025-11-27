using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // COIN
    private int score = 1000;
    [SerializeField] private TextMeshProUGUI scoreText;

    // ⭐ SKILL POINT
    private int skillPoints = 50;
    [SerializeField] private TextMeshProUGUI skillPointText;

    void Start()
    {
        UpdateScore();
        UpdateSkillPoints();
    }

    // ==============================
    //           SCORE
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
    //        SKILL POINT
    // ==============================
    public void AddSkillPoint(int amount)
    {
        skillPoints += amount;

        // ⭐ Luôn giữ skillPoints >= 0
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

}
