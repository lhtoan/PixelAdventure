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
    public bool receiveSkillPoint = true;

    [Tooltip("Số skill point nhận được khi mở chest (nếu nhận)")]
    public int skillPointReward = 1;

    [SerializeField] private GameManager gameManager;
    public ItemTreasureUI treasureUI;

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

        float dist = Vector2.Distance(player.position, transform.position);

        // ⛔ Nếu minigame đang chạy → không cho F xuất hiện
        if (isPlayingMinigame)
        {
            if (buttonF.activeSelf)
                buttonF.SetActive(false);

            return;
        }

        // 👉 Nếu PLAYER trong phạm vi mở
        if (dist <= interactRange)
        {
            ShowButtonF();

            if (Input.GetKeyDown(KeyCode.F))
            {
                isPlayingMinigame = true;

                // Mở minigame
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

    void ShowButtonF()
    {
        if (buttonF == null) return;

        buttonF.SetActive(true);
        buttonF.transform.position =
            player.position.x < transform.position.x ?
            transform.position + leftOffset :
            transform.position + rightOffset;
    }

    // 📌 Gọi khi thắng minigame → mở rương
    public void OpenChest()
    {
        alreadyOpened = true;

        if (anim != null)
            anim.SetTrigger("open");

        PlayerSkill ps = player.GetComponent<PlayerSkill>();

        int rewardCoin = Random.Range(minCoin, maxCoin + 1);
        rewardCoin = ps.ApplyTreasureBonus(rewardCoin);

        int rewardSkill = receiveSkillPoint ? ps.ApplyTreasureBonus(skillPointReward) : 0;

        if (gameManager != null)
        {
            gameManager.AddScore(rewardCoin);

            if (receiveSkillPoint)
                gameManager.AddSkillPoint(rewardSkill);
        }

        if (treasureUI != null)
            treasureUI.Show(rewardCoin, rewardSkill);
    }

    // 📌 Gọi khi minigame đóng mà KHÔNG thắng → cho phép mở lại rương
    public void OnMinigameClosedWithoutSuccess()
    {
        if (!alreadyOpened)
        {
            isPlayingMinigame = false;  // ⭐ Cho phép F hoạt động lại
        }
    }
}
