using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UI_SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public PlayerSkill.SkillType skillType;

    public Image unblockedImage;
    public Image blockedImage;

    private RectTransform rt;
    private List<UILineConnector> incomingLines = new();
    private List<UILineConnector> outgoingLines = new();

    // double click
    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 0.3f;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        CacheIncomingLines();
    }

    void CacheIncomingLines()
    {
        incomingLines.Clear();
        outgoingLines.Clear();

        UILineConnector[] allLines = FindObjectsByType<UILineConnector>(FindObjectsSortMode.None);

        foreach (var line in allLines)
        {
            if (line.to == rt)
                incomingLines.Add(line);

            if (line.from == rt)
                outgoingLines.Add(line);
        }
    }

    public void SetUnlocked(bool unlocked)
    {
        if (unblockedImage) unblockedImage.gameObject.SetActive(unlocked);
        if (blockedImage) blockedImage.gameObject.SetActive(!unlocked);

        foreach (var line in incomingLines)
            line.SetUnlocked(unlocked);
    }

    // CLICK HANDLER
    // public void OnPointerClick(PointerEventData eventData)
    // {
    //     if (Time.unscaledTime - lastClickTime < doubleClickThreshold)
    //     {
    //         // DOUBLE CLICK → mở skill
    //         UI_SkillTree.Instance.Unlock(skillType);
    //     }
    //     else
    //     {
    //         // SINGLE CLICK → mô tả skill + cost
    //         ShowSkillInfo();
    //     }

    //     lastClickTime = Time.unscaledTime;
    // }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Time.unscaledTime - lastClickTime < doubleClickThreshold)
        {
            // DOUBLE CLICK → mở skill
            UI_SkillTree.Instance.Unlock(skillType);
        }
        else
        {
            // SINGLE CLICK → hiện mô tả
            ShowSkillInfo();
        }

        lastClickTime = Time.unscaledTime;
    }


    // private void ShowSkillInfo()
    // {
    //     var tree = UI_SkillTree.Instance;
    //     var playerSkill = tree.GetPlayerSkill();   // ✔ FIXED

    //     if (playerSkill == null)
    //         return;

    //     string desc = playerSkill.GetSkillDescription(skillType);

    //     int sp = 0, coin = 0;
    //     tree.GetCosts(skillType, ref sp, ref coin);

    //     Debug.Log(
    //         $"📘 SKILL INFO: {skillType}\n" +
    //         $"➡ Mô tả: {desc}\n" +
    //         $"➡ Cost: {sp} Skill Point, {coin} Coin\n" +
    //         $"(Double-click để mở)"
    //     );
    // }

    private void ShowSkillInfo()
    {
        var tree = UI_SkillTree.Instance;
        var playerSkill = tree.GetPlayerSkill();

        if (playerSkill == null)
            return;

        // ⭐ Hiện mô tả + cost trong UI
        tree.ShowSkillInfoUI(skillType);

        // log thêm nếu muốn
        string desc = playerSkill.GetSkillDescription(skillType);

        int sp = 0, coin = 0;
        tree.GetCosts(skillType, ref sp, ref coin);

        // Debug.Log(
        //     $"📘 SKILL INFO: {skillType}\n" +
        //     $"➡ Mô tả: {desc}\n" +
        //     $"➡ Cost: {sp} Skill Point, {coin} Coin\n" +
        //     $"(Double-click để mở)"
        // );
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        HighlightRecursive(skillType, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HighlightRecursive(skillType, false);
    }

    void HighlightRecursive(PlayerSkill.SkillType node, bool active)
    {
        UILineConnector[] allLines = FindObjectsByType<UILineConnector>(FindObjectsSortMode.None);

        foreach (var line in allLines)
        {
            UI_SkillButton toBtn = line.to.GetComponent<UI_SkillButton>();
            if (toBtn != null && toBtn.skillType == node)
            {
                line.Highlight(active);
            }
        }

        if (!UI_SkillTree.Instance.prerequisite.ContainsKey(node))
            return;

        foreach (var parent in UI_SkillTree.Instance.prerequisite[node])
        {
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

            HighlightRecursive(parent, active);
        }
    }
}
