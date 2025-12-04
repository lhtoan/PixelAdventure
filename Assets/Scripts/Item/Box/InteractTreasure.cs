using UnityEngine;

public class InteractwithMiniggame : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject buttonF;
    public Animator anim;
    public DrawMinigame minigame;

    [Header("Settings")]
    public float interactRange = 2f;

    [Header("Button Offset")]
    public Vector3 leftOffset = new Vector3(-1f, 0.5f, 0f);
    public Vector3 rightOffset = new Vector3(1f, 0.5f, 0f);

    private bool alreadyOpened = false;
    private bool isPlayingMinigame = false;

    [Header("Rewards")]
    public int minCoin = 50;
    public int maxCoin = 150;

    [Tooltip("Nhận skill point khi mở chest")]
    public bool receiveSkillPoint = true;  // ← THÊM GIÁ TRỊ BOOL

    [Tooltip("Số skill point nhận được khi mở chest (nếu nhận)")]
    public int skillPointReward = 1;

    [SerializeField] private GameManager gameManager;
    public ItemTreasureUI treasureUI;   // ← Kéo từ Inspector vào



    void Start()
    {
        if (buttonF != null)
            buttonF.SetActive(false);

        if (anim == null)
            anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (alreadyOpened) return;
        if (player == null) return;

        // ⛔ Đang chơi minigame → không cho hiện nút F nữa
        if (isPlayingMinigame)
        {
            if (buttonF.activeSelf)
                buttonF.SetActive(false);
            return;
        }

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
            {
                isPlayingMinigame = true;

                // 👉 Kích hoạt minigame
                minigame.OpenFromTreasure(this);

                if (buttonF != null)
                    buttonF.SetActive(false);
            }
        }
        else
        {
            if (buttonF != null)
                buttonF.SetActive(false);
        }
    }

    // 📌 Gọi khi minigame thắng
    // public void OpenChest()
    // {
    //     alreadyOpened = true;

    //     if (anim != null)
    //         anim.SetTrigger("open");

    //     PlayerSkill ps = player.GetComponent<PlayerSkill>();

    //     // 🎁 Coin reward
    //     int rewardCoin = Random.Range(minCoin, maxCoin + 1);
    //     rewardCoin = ps.ApplyTreasureBonus(rewardCoin);

    //     // 🎁 Skill reward (nếu bật)
    //     int rewardSkill = 0;

    //     if (receiveSkillPoint) // ← CHỈ NHẬN NẾU TICK
    //     {
    //         rewardSkill = ps.ApplyTreasureBonus(skillPointReward);
    //     }

    //     // ✔ Add vào GameManager
    //     if (gameManager != null)
    //     {
    //         gameManager.AddScore(rewardCoin);

    //         if (receiveSkillPoint)
    //             gameManager.AddSkillPoint(rewardSkill);
    //     }
    // }

    public void OpenChest()
    {
        alreadyOpened = true;

        if (anim != null)
            anim.SetTrigger("open");

        PlayerSkill ps = player.GetComponent<PlayerSkill>();

        // 🎁 Coin reward
        int rewardCoin = Random.Range(minCoin, maxCoin + 1);
        rewardCoin = ps.ApplyTreasureBonus(rewardCoin);

        // 🎁 Skill reward
        int rewardSkill = 0;
        if (receiveSkillPoint)
            rewardSkill = ps.ApplyTreasureBonus(skillPointReward);

        // ✔ Add vào GameManager
        if (gameManager != null)
        {
            gameManager.AddScore(rewardCoin);

            if (receiveSkillPoint)
                gameManager.AddSkillPoint(rewardSkill);
        }

        // ⭐⭐⭐ HIỆN UI TREASURE ⭐⭐⭐
        if (treasureUI != null)
        {
            treasureUI.Show(rewardCoin, rewardSkill);
        }
    }


    // 📌 Gọi khi minigame đóng nhưng *không thắng* → cho phép hiện F lại
    public void OnMinigameClosedWithoutSuccess()
    {
        if (!alreadyOpened)
            isPlayingMinigame = false;
    }
}
