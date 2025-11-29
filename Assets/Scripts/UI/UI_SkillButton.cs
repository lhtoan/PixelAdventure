using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UI_SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public PlayerSkill.SkillType skillType;

    public Image unblockedImage;
    public Image blockedImage;

    private RectTransform rt;
    private List<UILineConnector> incomingLines = new();
    private List<UILineConnector> outgoingLines = new();

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        CacheIncomingLines();
    }

    // Tìm line có to == node này
    void CacheIncomingLines()
    {
        incomingLines.Clear();
        outgoingLines.Clear(); // ★ THÊM

        UILineConnector[] allLines = FindObjectsByType<UILineConnector>(FindObjectsSortMode.None);

        foreach (var line in allLines)
        {
            if (line.to == rt)
                incomingLines.Add(line);

            if (line.from == rt)       // ★ THÊM: tìm line đi RA node này
                outgoingLines.Add(line);
        }
    }


    // Đổi màu khi unlock
    public void SetUnlocked(bool unlocked)
    {
        if (unblockedImage) unblockedImage.gameObject.SetActive(unlocked);
        if (blockedImage) blockedImage.gameObject.SetActive(!unlocked);

        foreach (var line in incomingLines)
            line.SetUnlocked(unlocked);
    }

    // ⭐⭐ HOVER → highlight toàn bộ đường dẫn mở node này
    public void OnPointerEnter(PointerEventData eventData)
    {
        HighlightRecursive(skillType, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HighlightRecursive(skillType, false);
    }

    // Đệ quy highlight theo prerequisite
    void HighlightRecursive(PlayerSkill.SkillType node, bool active)
    {
        // 1) Highlight tất cả line dẫn vào node (kể cả từ icon)
        UILineConnector[] allLines = FindObjectsByType<UILineConnector>(FindObjectsSortMode.None);

        foreach (var line in allLines)
        {
            UI_SkillButton toBtn = line.to.GetComponent<UI_SkillButton>();
            if (toBtn != null && toBtn.skillType == node)
            {
                line.Highlight(active);   // sáng line icon → skill
            }
        }

        // 2) Nếu node không có prerequisite, dừng
        if (!UI_SkillTree.Instance.prerequisite.ContainsKey(node))
            return;

        // 3) Lặp cha của node
        foreach (var parent in UI_SkillTree.Instance.prerequisite[node])
        {
            // highlight line CHA → NODE
            foreach (var line in allLines)
            {
                UI_SkillButton fromBtn = line.from.GetComponent<UI_SkillButton>();
                UI_SkillButton toBtn2 = line.to.GetComponent<UI_SkillButton>();

                if (fromBtn != null && toBtn2 != null &&
                    fromBtn.skillType == parent &&
                    toBtn2.skillType == node)
                {
                    line.Highlight(active);
                }
            }

            // Đệ quy CHA
            HighlightRecursive(parent, active);
        }
    }



}
