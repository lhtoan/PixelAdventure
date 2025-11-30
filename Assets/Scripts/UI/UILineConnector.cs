// using UnityEngine;
// using UnityEngine.UI;

// [RequireComponent(typeof(RectTransform), typeof(Image))]
// public class UILineConnector : MonoBehaviour
// {
//     public RectTransform from;   // Node cha
//     public RectTransform to;     // Node con

//     // Locked = xanh đen
//     public Color lockedColor = new Color(0f / 255f, 26f / 255f, 46f / 255f, 1f); // #001A2E

//     // Unlocked = đỏ đẹp #DC143C
//     public Color unlockedColor = new Color(220f / 255f, 20f / 255f, 60f / 255f, 1f);

//     public float thickness = 3f;

//     private RectTransform rt;
//     private Image img;

//     void Awake()
//     {
//         rt = GetComponent<RectTransform>();
//         img = GetComponent<Image>();

//         rt.pivot = new Vector2(0.5f, 0.5f);
//         rt.anchorMin = new Vector2(0.5f, 0.5f);
//         rt.anchorMax = new Vector2(0.5f, 0.5f);
//         rt.localScale = Vector3.one;

//         img.color = lockedColor; // luôn xuất hiện line locked
//     }

//     void LateUpdate()
//     {
//         if (from == null || to == null) return;

//         RectTransform parent = rt.parent as RectTransform;
//         if (parent == null) return;

//         Vector3 A = parent.InverseTransformPoint(from.position);
//         Vector3 B = parent.InverseTransformPoint(to.position);

//         Vector3 dir = B - A;
//         float dist = dir.magnitude;

//         rt.localPosition = (A + B) * 0.5f;
//         rt.sizeDelta = new Vector2(dist, thickness);

//         float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
//         rt.localRotation = Quaternion.Euler(0, 0, angle);
//     }

//     // Reset line khi mở UI
//     public void ResetLine()
//     {
//         img.color = lockedColor;
//     }

//     // Đổi màu khi mở skill
//     // public void SetUnlocked(bool unlocked)
//     // {
//     //     img.color = unlocked ? unlockedColor : lockedColor;
//     // }

//     public void Highlight(bool active)
//     {
//         if (active)
//         {
//             img.color = Color.Lerp(img.color, unlockedColor, 0.6f) + new Color(0.2f, 0.2f, 0.2f, 0.3f);
//         }
//         else
//         {
//             img.color = unlocked ? unlockedColor : lockedColor;
//         }
//     }


//     public bool unlocked = false; // thêm mới để lưu trạng thái

//     public void SetUnlocked(bool state)
//     {
//         unlocked = state;
//         img.color = unlocked ? unlockedColor : lockedColor;
//     }

// }
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Image))]
public class UILineConnector : MonoBehaviour
{
    public enum ElementType { Fire, Ice }
    public ElementType element = ElementType.Fire; // ⭐ mặc định Fire

    public RectTransform from;
    public RectTransform to;

    // Locked = xanh đen
    public Color lockedColor = new Color(0f / 255f, 26f / 255f, 46f / 255f, 1f); // #001A2E

    // Fire = đỏ
    public Color fireColor = new Color(220f / 255f, 20f / 255f, 60f / 255f, 1f); // #DC143C

    // Ice = xanh
    public Color iceColor = new Color(42f / 255f, 169f / 255f, 255f / 255f, 1f); // #2AA9FF

    public float thickness = 3f;

    private RectTransform rt;
    private Image img;

    public bool unlocked = false;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        img = GetComponent<Image>();

        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.localScale = Vector3.one;

        img.color = lockedColor; // mặc định locked
    }

    void LateUpdate()
    {
        if (from == null || to == null) return;

        RectTransform parent = rt.parent as RectTransform;
        if (parent == null) return;

        Vector3 A = parent.InverseTransformPoint(from.position);
        Vector3 B = parent.InverseTransformPoint(to.position);

        Vector3 dir = B - A;
        float dist = dir.magnitude;

        rt.localPosition = (A + B) * 0.5f;
        rt.sizeDelta = new Vector2(dist, thickness);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rt.localRotation = Quaternion.Euler(0, 0, angle);
    }

    // Reset line khi UI mở lại
    public void ResetLine()
    {
        unlocked = false;
        img.color = lockedColor;
    }

    // Set unlocked (màu theo hệ)
    public void SetUnlocked(bool state)
    {
        unlocked = state;

        if (!unlocked)
        {
            img.color = lockedColor;
            return;
        }

        // ⭐ Auto đổi màu theo hệ Fire / Ice
        img.color = (element == ElementType.Fire) ? fireColor : iceColor;
    }

    public void Highlight(bool active)
    {
        if (active)
        {
            Color baseCol = (element == ElementType.Fire) ? fireColor : iceColor;
            img.color = Color.Lerp(img.color, baseCol, 0.6f) + new Color(0.15f, 0.15f, 0.15f, 0.2f);
        }
        else
        {
            img.color = unlocked
                ? (element == ElementType.Fire ? fireColor : iceColor)
                : lockedColor;
        }
    }
}
