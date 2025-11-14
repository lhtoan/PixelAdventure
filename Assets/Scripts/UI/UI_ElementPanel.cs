using UnityEngine;
using System.Collections;

public class UI_ElementPanel : MonoBehaviour
{
    [Header("Element Icons")]
    [SerializeField] private CanvasGroup fireIcon;
    [SerializeField] private CanvasGroup iceIcon;

    [Header("Animation Settings")]
    [SerializeField] private float fadeTime = 0.2f;
    [SerializeField] private float scalePop = 1.2f;

    private void Start()
    {
        PlayerAttack player = FindFirstObjectByType<PlayerAttack>();

        if (player != null)
        {
            UpdateElement(player.CurrentElement, true); // load ban đầu ko hiệu ứng
        }
        else
        {
            Debug.LogWarning("⚠ Không tìm thấy PlayerAttack!");
        }
    }

    public void UpdateElement(PlayerAttack.Element element, bool instant = false)
    {
        StopAllCoroutines();

        if (element == PlayerAttack.Element.Fire)
        {
            StartCoroutine(SwitchIcon(fireIcon, iceIcon, instant));
        }
        else
        {
            StartCoroutine(SwitchIcon(iceIcon, fireIcon, instant));
        }
    }

    private IEnumerator SwitchIcon(CanvasGroup show, CanvasGroup hide, bool instant)
    {
        if (instant)
        {
            // Set ban đầu không hiệu ứng
            show.alpha = 1;
            show.transform.localScale = Vector3.one;
            hide.alpha = 0;
            hide.transform.localScale = Vector3.one;
            yield break;
        }

        // Fade out icon cũ
        float t = 0;
        Vector3 hideStartScale = hide.transform.localScale;

        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            float p = t / fadeTime;

            hide.alpha = Mathf.Lerp(1, 0, p);
            hide.transform.localScale = Vector3.Lerp(hideStartScale, Vector3.one * 0.8f, p);

            yield return null;
        }

        // Reset icon mới
        show.alpha = 0;
        show.transform.localScale = Vector3.one * scalePop;

        // Fade-in icon mới + pop animation
        t = 0;
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            float p = t / fadeTime;

            show.alpha = Mathf.Lerp(0, 1, p);
            show.transform.localScale = Vector3.Lerp(Vector3.one * scalePop, Vector3.one, p);

            yield return null;
        }
    }
}
