// using UnityEngine;

// public class Mushroom : MonoBehaviour
// {
//     [Header("Patrol Points")]
//     [SerializeField] private Transform posA;
//     [SerializeField] private Transform posB;

//     [Header("Movement Settings")]
//     [SerializeField] private float speed = 2f;
//     [SerializeField] private float idleDuration = 0.5f;

//     [Header("Mushroom Components")]
//     [SerializeField] private Animator anim;

//     private Vector3 initScale;
//     private bool movingLeft = true;
//     private float idleTimer;
//     private bool isDead = false;

//     private void Awake()
//     {
//         initScale = transform.localScale;
//     }

//     private void Update()
//     {
//         if (isDead) return;
//         Patrol();
//     }

//     private void Patrol()
//     {
//         if (movingLeft)
//         {
//             if (transform.position.x > posA.position.x)
//                 MoveInDirection(-1);
//             else
//                 ChangeDirection();
//         }
//         else
//         {
//             if (transform.position.x < posB.position.x)
//                 MoveInDirection(1);
//             else
//                 ChangeDirection();
//         }
//     }

//     private void ChangeDirection()
//     {
//         idleTimer += Time.deltaTime;
//         if (idleTimer >= idleDuration)
//         {
//             movingLeft = !movingLeft;
//             idleTimer = 0;
//         }
//     }

//     private void MoveInDirection(int dir)
//     {
//         idleTimer = 0;

//         // ✅ Xoay mặt theo hướng di chuyển
//         transform.localScale = new Vector3(
//             Mathf.Abs(initScale.x) * -dir,
//             initScale.y,
//             initScale.z
//         );

//         // ✅ Di chuyển
//         transform.position = new Vector3(
//             transform.position.x + Time.deltaTime * dir * speed,
//             transform.position.y,
//             transform.position.z
//         );
//     }

//     // 🧠 Gọi khi Mushroom bị giết (từ script Health)
//     public void Die()
//     {
//         if (isDead) return;
//         isDead = true;

//         anim.SetTrigger("die");

//         // Ngừng di chuyển & tắt collider nếu cần
//         GetComponent<Collider2D>().enabled = false;

//         // Xóa sau 1.5s (đủ thời gian chơi animation chết)
//         Destroy(gameObject, 1.5f);
//     }
// }
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] private Transform posA;
    [SerializeField] private Transform posB;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float idleDuration = 0.5f;
    [SerializeField] private bool startMovingLeft = true; // ⭐ hướng khởi động

    [Header("Mushroom Components")]
    [SerializeField] private Animator anim;

    private Vector3 initScale;
    private bool movingLeft;
    private float idleTimer;
    private bool isDead = false;

    private void Awake()
    {
        initScale = transform.localScale;
        movingLeft = startMovingLeft; // ⭐ hướng ban đầu
    }

    private void Update()
    {
        if (isDead) return;
        Patrol();
    }

    private void Patrol()
    {
        float left = Mathf.Min(posA.position.x, posB.position.x);
        float right = Mathf.Max(posA.position.x, posB.position.x);

        if (movingLeft)
        {
            if (transform.position.x > left)
                MoveInDirection(-1);
            else
                ChangeDirection();
        }
        else
        {
            if (transform.position.x < right)
                MoveInDirection(1);
            else
                ChangeDirection();
        }
    }

    private void ChangeDirection()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleDuration)
        {
            movingLeft = !movingLeft;
            idleTimer = 0;
        }
    }

    private void MoveInDirection(int dir)
    {
        idleTimer = 0;

        transform.localScale = new Vector3(
            Mathf.Abs(initScale.x) * -dir,
            initScale.y,
            initScale.z
        );

        transform.position = new Vector3(
            transform.position.x + Time.deltaTime * dir * speed,
            transform.position.y,
            transform.position.z
        );
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        anim.SetTrigger("die");

        GetComponent<Collider2D>().enabled = false;

        Destroy(gameObject, 1.5f);
    }
}
