// using UnityEngine;

// public class Spikehead : EnemyDamage
// {
//     [Header("SpikeHead Attributes")]
//     [SerializeField] private float speed;
//     [SerializeField] private float range;
//     [SerializeField] private float checkDelay;
//     [SerializeField] private LayerMask playerLayer;

//     [Header("Activation Settings")]
//     [SerializeField] private int activationLimit = 1; 
//     private int currentActivations = 0;

//     private Vector3 moveDirection;
//     private Vector3[] directions = new Vector3[4];
//     private float checkTimer;
//     private bool attacking;
//     private bool hasStoppedPermanently;

//     private void OnEnable()
//     {
//         StopCompletely();
//     }

//     private void Update()
//     {
//         if (hasStoppedPermanently)
//             return;

//         if (attacking)
//         {
//             transform.Translate(moveDirection * Time.deltaTime * speed);
//         }
//         else
//         {
//             checkTimer += Time.deltaTime;
//             if (checkTimer >= checkDelay)
//                 CheckForPlayer();
//         }
//     }

//     private void CheckForPlayer()
//     {
//         // Nếu vượt quá số lần cho phép → dừng hẳn
//         if (currentActivations >= activationLimit)
//         {
//             hasStoppedPermanently = true;
//             return;
//         }

//         CalculateDirections();

//         // Kiểm tra 4 hướng xem có Player không
//         for (int i = 0; i < directions.Length; i++)
//         {
//             Debug.DrawRay(transform.position, directions[i] * range, Color.red);
//             RaycastHit2D hit = Physics2D.Raycast(transform.position, directions[i], range, playerLayer);

//             if (hit.collider != null && !attacking)
//             {
//                 attacking = true;
//                 moveDirection = directions[i].normalized;
//                 checkTimer = 0;
//                 currentActivations++; //tăng số lần hoạt động
//                 break;
//             }
//         }
//     }

//     private void CalculateDirections()
//     {
//         directions[0] = transform.right;  // phải
//         directions[1] = -transform.right; // trái
//         directions[2] = transform.up;     // lên
//         directions[3] = -transform.up;    // xuống
//     }

//     private void Stop()
//     {
//         attacking = false;
//         moveDirection = Vector3.zero;
//     }

//     private void StopCompletely()
//     {
//         Stop();
//         hasStoppedPermanently = false;
//         currentActivations = 0; // reset số lần nếu được bật lại từ Pool
//     }

//     protected override void OnTriggerEnter2D(Collider2D collision)
//     {
//         // Nếu chạm Player → gây damage + dừng
//         if (collision.CompareTag("Player"))
//         {
//             base.OnTriggerEnter2D(collision);
//             StopTemporarilyOrForever();
//         }
//         // Nếu chạm Ground → dừng
//         else if (collision.CompareTag("Ground"))
//         {
//             StopTemporarilyOrForever();
//         }
//     }

//     // Nếu còn lượt thì chỉ dừng tạm thời, hết lượt thì dừng vĩnh viễn
//     private void StopTemporarilyOrForever()
//     {
//         Stop();
//         if (currentActivations >= activationLimit)
//         {
//             hasStoppedPermanently = true;
//         }
//     }
// }
using UnityEngine;

public class Spikehead : EnemyDamage
{
    public enum SpikeDirection
    {
        Right,
        Left,
        Up,
        Down
    }

    [Header("SpikeHead Attributes")]
    [SerializeField] private float speed;
    [SerializeField] private float range;
    [SerializeField] private float checkDelay;
    [SerializeField] private LayerMask playerLayer;

    [Header("Detection Direction")]
    [SerializeField] private SpikeDirection watchDirection;

    [Header("Activation Settings")]
    [SerializeField] private int activationLimit = 1;
    private int currentActivations = 0;

    private Vector3 moveDirection;
    private float checkTimer;
    private bool attacking;
    private bool hasStoppedPermanently;

    private void OnEnable()
    {
        StopCompletely();
    }

    private void Update()
    {
        if (hasStoppedPermanently)
            return;

        if (attacking)
        {
            transform.Translate(moveDirection * Time.deltaTime * speed);
        }
        else
        {
            checkTimer += Time.deltaTime;
            if (checkTimer >= checkDelay)
                CheckForPlayer();
        }
    }

    private void CheckForPlayer()
    {
        if (currentActivations >= activationLimit)
        {
            hasStoppedPermanently = true;
            return;
        }

        Vector3 dir = GetWatchDirection();

        Debug.DrawRay(transform.position, dir * range, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, range, playerLayer);

        if (hit.collider != null && !attacking)
        {
            attacking = true;
            moveDirection = dir.normalized;
            checkTimer = 0;
            currentActivations++;
        }
    }

    private Vector3 GetWatchDirection()
    {
        switch (watchDirection)
        {
            case SpikeDirection.Right: return transform.right;
            case SpikeDirection.Left: return -transform.right;
            case SpikeDirection.Up: return transform.up;
            case SpikeDirection.Down: return -transform.up;
        }
        return transform.right;
    }

    private void Stop()
    {
        attacking = false;
        moveDirection = Vector3.zero;
    }

    private void StopCompletely()
    {
        Stop();
        hasStoppedPermanently = false;
        currentActivations = 0;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            base.OnTriggerEnter2D(collision);
            StopTemporarilyOrForever();
        }
        else if (collision.CompareTag("Ground"))
        {
            StopTemporarilyOrForever();
        }
    }

    private void StopTemporarilyOrForever()
    {
        Stop();

        if (currentActivations >= activationLimit)
        {
            hasStoppedPermanently = true;
        }

        // ⭐ NEW: Biến mất sau khi va chạm ⭐
        gameObject.SetActive(false);
    }
}
