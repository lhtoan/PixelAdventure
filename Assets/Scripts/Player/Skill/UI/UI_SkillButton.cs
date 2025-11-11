using UnityEngine;
using UnityEngine.UI;

public class UI_SkillButton : MonoBehaviour
{
    public Image unblockedImage;   // ảnh màu
    public Image blockedImage;     // ảnh trắng đen

    public void SetUnlocked(bool unlocked)
    {
        if (unblockedImage != null)
            unblockedImage.gameObject.SetActive(unlocked);

        if (blockedImage != null)
            blockedImage.gameObject.SetActive(!unlocked);
    }
}
