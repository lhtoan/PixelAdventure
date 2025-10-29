using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float bounce = 10f;
    [SerializeField] private float resetTime = 0.3f; // thời gian bật xong

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            anim.SetBool("isOn", true);

            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Dùng linearVelocity thay vì velocity
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.AddForce(Vector2.up * bounce, ForceMode2D.Impulse);
            }

            Invoke(nameof(ResetPad), resetTime);
        }
    }

    private void ResetPad()
    {
        anim.SetBool("isOn", false);
    }
}
