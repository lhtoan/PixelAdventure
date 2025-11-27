using UnityEngine;
using System.Collections;

public class PlayerStatus : MonoBehaviour
{
    public bool isStunned = false;

    // GIỮ = 0 ĐỂ STUN ĐÚNG 2 GIÂY
    public float stunReductionPercent = 0f;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void ApplyStun(float duration)
    {
        if (isStunned) return;

        float finalDuration = duration; // ⭐ luôn = 2 giây (hoặc duration truyền vào)

        Debug.Log($"[PlayerStatus] STUN START | Duration = {finalDuration:F2}s");

        StartCoroutine(StunCoroutine(finalDuration));
    }

    private IEnumerator StunCoroutine(float t)
    {
        isStunned = true;
        anim.SetBool("isStun", true);

        yield return new WaitForSeconds(t);

        anim.SetBool("isStun", false);
        isStunned = false;

        Debug.Log("[PlayerStatus] STUN END");
    }
}
