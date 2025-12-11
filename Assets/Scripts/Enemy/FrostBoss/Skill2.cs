using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossSkill2 : MonoBehaviour
{
    [Header("Spike Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private List<GameObject> spikePool;

    [SerializeField] private float spikeDistance = 1.6f;
    [SerializeField] private float spikeLifetime = 1.2f;

    [Header("Boss Jump Settings")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private Transform groundCheck;      // ⭐ KÉO VÀO ĐÂY
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;      // ⭐ GROUND

    [SerializeField] private Rigidbody2D rb;

    public bool IsCasting { get; private set; }

    private void Awake()
    {
        if (rb == null)
            rb = GetComponentInParent<Rigidbody2D>();

        if (rb == null)
            Debug.LogError("❌ BossSkill2: Missing Rigidbody2D!");

        if (groundCheck == null)
            Debug.LogError("❌ BossSkill2: Missing GroundCheck transform!");

        foreach (var spike in spikePool)
            if (spike != null) spike.SetActive(false);
    }

    public IEnumerator CastSkill2()
    {
        if (IsCasting) yield break;
        IsCasting = true;

        // Debug.Log("Skill2 START");

        // ⭐ 1) Boss nhảy lên
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        // ⭐ 2) Đợi rơi xuống
        yield return StartCoroutine(WaitUntilFall());

        // ⭐ 3) Đợi chạm đất bằng GroundCheck
        yield return StartCoroutine(WaitUntilGrounded());

        // ⭐ 4) Bắn spike
        SpawnSpikes();

        yield return new WaitForSeconds(spikeLifetime);

        ClearSpikes();

        // Debug.Log("Skill2 END");

        IsCasting = false;
    }

    // ----------------- CHECK GROUNDED -----------------

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    private IEnumerator WaitUntilGrounded()
    {
        while (!IsGrounded())
            yield return null;
    }

    private IEnumerator WaitUntilFall()
    {
        while (rb.linearVelocity.y > 0)
            yield return null;
    }

    // ----------------- SPAWN LOGIC -----------------

    private void SpawnSpikes()
    {
        Vector3 origin = spawnPoint.position;
        int half = spikePool.Count / 2;

        // Middle spike
        SetupSpike(spikePool[0], origin);

        // Left + Right
        for (int i = 1; i <= half; i++)
        {
            float dist = i * spikeDistance;

            if (i < spikePool.Count)
                SetupSpike(spikePool[i], origin + Vector3.left * dist);

            int ri = i + half;
            if (ri < spikePool.Count)
                SetupSpike(spikePool[ri], origin + Vector3.right * dist);
        }
    }

    private void SetupSpike(GameObject spike, Vector3 pos)
    {
        if (spike == null) return;

        spike.transform.position = pos;
        spike.SetActive(true);
    }

    private void ClearSpikes()
    {
        foreach (var s in spikePool)
            if (s != null) s.SetActive(false);
    }
}
