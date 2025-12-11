using UnityEngine;
using System.Collections;

public class PlayerClimb : MonoBehaviour
{
    [Header("Climb Settings")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float climbSpeed = 8f;

    private float vertical;
    private bool isLadder;
    private bool isClimbing;
    private PlatformEffector2D currentEffector;

    void Update()
    {
        vertical = Input.GetAxisRaw("Vertical");

        // Bắt đầu leo khi ở trong vùng thang và nhấn lên/xuống
        if (isLadder && Mathf.Abs(vertical) > 0.1f)
            isClimbing = true;

        // Đang đứng trên platform và nhấn xuống → tụt xuống
        if (currentEffector && vertical < 0)
            StartCoroutine(GoDownLadder());

        // Đang ở vùng thang và nhấn lên → leo xuyên platform
        if (currentEffector && isLadder && vertical > 0.1f)
            StartCoroutine(GoUpThroughGround());
    }

    private void FixedUpdate()
    {
        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, vertical * climbSpeed);
        }
        else
        {
            rb.gravityScale = 4f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
            isLadder = true;

        if (collision.CompareTag("Ground"))
            currentEffector = collision.GetComponent<PlatformEffector2D>();

        if (collision.CompareTag("LadderTopTrigger"))
            StartCoroutine(ExitClimbSmooth());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = false;
            isClimbing = false;
        }

        if (collision.CompareTag("Ground"))
            currentEffector = null;
    }

    // Dừng leo khi lên tới đỉnh
    private IEnumerator ExitClimbSmooth()
    {
        yield return new WaitForSeconds(0.1f);
        isClimbing = false;
        rb.gravityScale = 4f;
        rb.linearVelocity = Vector2.zero;
        rb.position += Vector2.up * 0.1f;
    }

    // Tụt xuống thang qua platform
    private IEnumerator GoDownLadder()
    {
        if (currentEffector == null) yield break;

        var eff = currentEffector;
        eff.rotationalOffset = 180f;
        yield return new WaitForSeconds(0.3f);

        if (eff != null)
            eff.rotationalOffset = 0f;
    }

    // Leo xuyên lên qua platform
    private IEnumerator GoUpThroughGround()
    {
        if (currentEffector == null) yield break;

        var eff = currentEffector;
        eff.rotationalOffset = 180f;
        yield return new WaitForSeconds(0.3f);

        if (eff != null)
            eff.rotationalOffset = 0f;
    }
}

