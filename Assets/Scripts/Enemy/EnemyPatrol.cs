// using UnityEngine;

// public class EnemyPatrol : MonoBehaviour, IEnemyMovement
// {
//     [Header("Patrol Points")]
//     [SerializeField] public Transform leftEdge;
//     [SerializeField] public Transform rightEdge;

//     [Header("Enemy")]
//     [SerializeField] private Transform enemy;

//     [Header("Movement parameters")]
//     [SerializeField] private float speed;
//     private Vector3 initScale;
//     private bool movingLeft;

//     [Header("Idle Behaviour")]
//     [SerializeField] private float idleDuration;
//     private float idleTimer;

//     [Header("Enemy Animator")]
//     [SerializeField] private Animator anim;
//     [HideInInspector] public bool isFrozen = false;


//     public void EnableMovement(bool enable)
//     {
//         this.enabled = enable;
//     }

//     private void Awake()
//     {
//         initScale = enemy.localScale;
//     }
//     private void OnDisable()
//     {
//         anim.SetBool("moving", false);
//     }

//     // private void Update()
//     // {
//     //     if (movingLeft)
//     //     {
//     //         if (enemy.position.x >= leftEdge.position.x)
//     //             MoveInDirection(-1);
//     //         else
//     //             DirectionChange();
//     //     }
//     //     else
//     //     {
//     //         if (enemy.position.x <= rightEdge.position.x)
//     //             MoveInDirection(1);
//     //         else
//     //             DirectionChange();
//     //     }
//     // }
//     private void Update()
//     {
//         if (isFrozen)
//         {
//             anim.SetBool("moving", false);
//             return; // ❄️ Không di chuyển khi bị đóng băng
//         }

//         if (movingLeft)
//         {
//             if (enemy.position.x >= leftEdge.position.x)
//                 MoveInDirection(-1);
//             else
//                 DirectionChange();
//         }
//         else
//         {
//             if (enemy.position.x <= rightEdge.position.x)
//                 MoveInDirection(1);
//             else
//                 DirectionChange();
//         }
//     }


//     private void DirectionChange()
//     {
//         anim.SetBool("moving", false);
//         idleTimer += Time.deltaTime;

//         if (idleTimer > idleDuration)
//             movingLeft = !movingLeft;
//     }

//     private void MoveInDirection(int _direction)
//     {
//         idleTimer = 0;
//         anim.SetBool("moving", true);

//         //Make enemy face direction
//         enemy.localScale = new Vector3(Mathf.Abs(initScale.x) * _direction,
//             initScale.y, initScale.z);

//         //Move in that direction
//         enemy.position = new Vector3(enemy.position.x + Time.deltaTime * _direction * speed,
//             enemy.position.y, enemy.position.z);
//     }

//     public void SetFrozen(bool frozen)
//     {
//         isFrozen = frozen;
//         anim.SetBool("moving", !frozen);
//     }
// }
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] public Transform leftEdge;
    [SerializeField] public Transform rightEdge;

    [Header("Enemy")]
    [SerializeField] private Transform enemy;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float idleDuration = 0.5f;
    [SerializeField] private bool startMovingLeft = true;   // ⭐ thêm lựa chọn hướng khởi động

    [Header("Enemy Animator")]
    [SerializeField] private Animator anim;

    private bool movingLeft;
    private float idleTimer;
    private Vector3 initScale;

    public bool isFrozen = false;

    private void Awake()
    {
        initScale = enemy.localScale;
        movingLeft = startMovingLeft;   // ⭐ gán hướng ban đầu từ inspector
    }

    private void OnDisable()
    {
        anim.SetBool("moving", false);
    }

    private void Update()
    {
        if (isFrozen)
        {
            anim.SetBool("moving", false);
            return;
        }

        Patrol();
    }

    private void Patrol()
    {
        float left = leftEdge.position.x;
        float right = rightEdge.position.x;

        if (movingLeft)
        {
            if (enemy.position.x > left)
                MoveInDirection(-1);
            else
                ChangeDirection();
        }
        else
        {
            if (enemy.position.x < right)
                MoveInDirection(1);
            else
                ChangeDirection();
        }
    }

    private void ChangeDirection()
    {
        anim.SetBool("moving", false);
        idleTimer += Time.deltaTime;

        if (idleTimer >= idleDuration)
        {
            movingLeft = !movingLeft;
            idleTimer = 0f;
        }
    }

    private void MoveInDirection(int dir)
    {
        idleTimer = 0;
        anim.SetBool("moving", true);

        // ⭐ xoay enemy theo hướng đi
        enemy.localScale = new Vector3(
            Mathf.Abs(initScale.x) * dir,
            initScale.y,
            initScale.z
        );

        enemy.position += new Vector3(dir * speed * Time.deltaTime, 0f, 0f);
    }

    public void SetFrozen(bool frozen)
    {
        isFrozen = frozen;
        anim.SetBool("moving", !frozen);
    }
}
