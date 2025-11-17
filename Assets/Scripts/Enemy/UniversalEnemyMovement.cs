// using UnityEngine;

// public class UniversalEnemyMovement : MonoBehaviour, IEnemyMovement
// {
//     [Header("Target Settings")]
//     public Transform target;

//     [Header("Movement Settings")]
//     public float moveSpeed = 2f;
//     public bool isFlying = false;

//     [Header("Flying Attack Settings")]
//     public bool diveAttack = false;              // ⭐ Thêm: enemy bay lao vào target
//     public float diveSpeedMultiplier = 3f;       // ⭐ tốc độ lao mạnh

//     [Header("Flip Settings")]
//     public bool flipInverted = false;            // ⭐ ONLY mushroom bật cái này

//     [Header("Follow Range")]
//     public float followRange = 5f;
//     public float stopDistance = 0.5f;

//     [Header("Components")]
//     public Transform enemyModel;
//     public Animator anim;

//     [HideInInspector] public bool isFrozen = false;

//     private Vector3 initScale;

//     public void EnableMovement(bool enable)
//     {
//         this.enabled = enable;
//     }

//     private void Awake()
//     {
//         if (enemyModel == null) enemyModel = transform;
//         initScale = enemyModel.localScale;
//     }

//     private void Update()
//     {
//         if (isFrozen)
//         {
//             if (HasParameter("moving"))
//                 anim.SetBool("moving", false);
//             return;
//         }

//         if (target == null)
//         {
//             if (HasParameter("moving"))
//                 anim.SetBool("moving", false);
//             return;
//         }

//         float distance = Vector2.Distance(transform.position, target.position);

//         if (distance > followRange)
//         {
//             if (HasParameter("moving"))
//                 anim.SetBool("moving", false);
//             return;
//         }

//         if (distance < stopDistance)
//         {
//             if (HasParameter("moving"))
//                 anim.SetBool("moving", false);
//             return;
//         }

//         MoveToTarget();
//     }

//     private void MoveToTarget()
//     {
//         float direction = target.position.x - transform.position.x;

//         // ⭐ Flip đúng hướng (hoặc ngược nếu flipInverted)
//         if (direction != 0)
//         {
//             float flipDir = Mathf.Sign(direction);

//             if (flipInverted)
//                 flipDir = -flipDir;

//             enemyModel.localScale = new Vector3(
//                 Mathf.Abs(initScale.x) * flipDir,
//                 initScale.y,
//                 initScale.z
//             );
//         }

//         if (HasParameter("moving"))
//             anim.SetBool("moving", true);

//         Vector3 moveDir = (target.position - transform.position).normalized;

//         // ⭐⭐ NÂNG CẤP FLYING + DIVE ATTACK ⭐⭐
//         if (isFlying)
//         {
//             if (diveAttack)
//             {
//                 // 🦅 Enemy bay lao vào target nhanh hơn
//                 transform.position += moveDir * moveSpeed * diveSpeedMultiplier * Time.deltaTime;
//             }
//             else
//             {
//                 // Bay bình thường
//                 transform.position += moveDir * moveSpeed * Time.deltaTime;
//             }
//         }
//         else
//         {
//             // Enemy chạy dưới đất
//             transform.position +=
//                 new Vector3(Mathf.Sign(direction) * moveSpeed * Time.deltaTime, 0, 0);
//         }
//     }

//     private void OnDisable()
//     {
//         if (HasParameter("moving"))
//             anim.SetBool("moving", false);
//     }

//     public void SetFrozen(bool frozen)
//     {
//         isFrozen = frozen;
//         if (HasParameter("moving"))
//             anim.SetBool("moving", !frozen);
//     }

//     private bool HasParameter(string paramName)
//     {
//         foreach (AnimatorControllerParameter param in anim.parameters)
//         {
//             if (param.name == paramName)
//                 return true;
//         }
//         return false;
//     }
// }



// using UnityEngine;

// public class UniversalEnemyMovement : MonoBehaviour, IEnemyMovement
// {
//     [Header("Target Settings")]
//     public Transform target;

//     [Header("Movement Settings")]
//     public float moveSpeed = 2f;
//     public bool isFlying = false;

//     [Header("Flying Attack Settings")]
//     public bool diveAttack = false;
//     public float diveSpeedMultiplier = 3f;

//     [Header("Flip Settings")]
//     public bool flipInverted = false;
//     public float flipBuffer = 0.2f;   // ⭐ chống flip lung tung khi quá gần

//     [Header("Follow Range")]
//     public float followRange = 5f;
//     public float stopDistance = 0.5f;

//     [Header("Components")]
//     public Transform enemyModel;
//     public Animator anim;

//     [HideInInspector] public bool isFrozen = false;

//     private Vector3 initScale;

//     public void EnableMovement(bool enable) => this.enabled = enable;

//     private void Awake()
//     {
//         if (enemyModel == null) enemyModel = transform;
//         initScale = enemyModel.localScale;
//     }

//     private void Update()
//     {
//         if (isFrozen)
//         {
//             SetAnimMoving(false);
//             return;
//         }

//         if (target == null)
//         {
//             SetAnimMoving(false);
//             return;
//         }

//         float distance = Vector2.Distance(transform.position, target.position);

//         if (distance > followRange)
//         {
//             SetAnimMoving(false);
//             return;
//         }

//         if (distance < stopDistance)
//         {
//             SetAnimMoving(false);
//             return;
//         }

//         MoveToTarget();
//     }

//     private void MoveToTarget()
//     {
//         float direction = target.position.x - transform.position.x;
//         float distance = Mathf.Abs(direction);

//         // ⭐⭐⭐ LOGIC CHỐNG FLIP KHI QUÁ GẦN ⭐⭐⭐
//         if (distance > stopDistance + flipBuffer)
//         {
//             if (direction != 0)
//             {
//                 float flipDir = Mathf.Sign(direction);
//                 if (flipInverted) flipDir = -flipDir;

//                 enemyModel.localScale = new Vector3(
//                     Mathf.Abs(initScale.x) * flipDir,
//                     initScale.y,
//                     initScale.z
//                 );
//             }
//         }
//         // nếu trong khoảng stopDistance → KHÔNG flip

//         SetAnimMoving(true);

//         Vector3 moveDir = (target.position - transform.position).normalized;

//         if (isFlying)
//         {
//             float speedToUse = diveAttack ? moveSpeed * diveSpeedMultiplier : moveSpeed;
//             transform.position += moveDir * speedToUse * Time.deltaTime;
//         }
//         else
//         {
//             transform.position +=
//                 new Vector3(Mathf.Sign(direction) * moveSpeed * Time.deltaTime, 0, 0);
//         }
//     }

//     private void SetAnimMoving(bool state)
//     {
//         if (HasParameter("moving"))
//             anim.SetBool("moving", state);
//     }

//     private bool HasParameter(string paramName)
//     {
//         foreach (AnimatorControllerParameter param in anim.parameters)
//             if (param.name == paramName) return true;
//         return false;
//     }
// }
using UnityEngine;

public class UniversalEnemyMovement : MonoBehaviour, IEnemyMovement
{
    [Header("Target Settings")]
    public Transform target;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public bool isFlying = false;

    [Header("Flying Attack Settings")]
    public bool diveAttack = false;
    public float diveSpeedMultiplier = 3f;

    [Header("Flip Settings")]
    public bool flipInverted = false;
    public float flipBuffer = 0.2f;   // ⭐ thêm buffer chống flip khi gần target

    [Header("Follow Range")]
    public float followRange = 5f;
    public float stopDistance = 0.5f;

    [Header("Components")]
    public Transform enemyModel;
    public Animator anim;

    [HideInInspector] public bool isFrozen = false;

    private Vector3 initScale;

    public void EnableMovement(bool enable)
    {
        this.enabled = enable;
    }

    private void Awake()
    {
        if (enemyModel == null) enemyModel = transform;
        initScale = enemyModel.localScale;
    }

    private void Update()
    {
        if (isFrozen)
        {
            if (HasParameter("moving"))
                anim.SetBool("moving", false);
            return;
        }

        if (target == null)
        {
            if (HasParameter("moving"))
                anim.SetBool("moving", false);
            return;
        }

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance > followRange)
        {
            if (HasParameter("moving"))
                anim.SetBool("moving", false);
            return;
        }

        if (distance < stopDistance)
        {
            if (HasParameter("moving"))
                anim.SetBool("moving", false);
            return;
        }

        MoveToTarget();
    }

    private void MoveToTarget()
    {
        float directionX = target.position.x - transform.position.x;
        float absDistanceX = Mathf.Abs(directionX);

        // ⭐⭐⭐ LOGIC CHỐNG FLIP KHI QUÁ GẦN — CHỈ DÙNG CHO FLYING ⭐⭐⭐
        bool allowFlip = true;
        if (isFlying)
        {
            if (absDistanceX < stopDistance + flipBuffer)
                allowFlip = false;
        }

        // ⭐ Flip hướng (nếu được phép)
        if (directionX != 0 && allowFlip)
        {
            float flipDir = Mathf.Sign(directionX);
            if (flipInverted)
                flipDir = -flipDir;

            enemyModel.localScale = new Vector3(
                Mathf.Abs(initScale.x) * flipDir,
                initScale.y,
                initScale.z
            );
        }

        if (HasParameter("moving"))
            anim.SetBool("moving", true);

        Vector3 moveDir = (target.position - transform.position).normalized;

        // ⭐⭐ flying enemy logic giữ nguyên
        if (isFlying)
        {
            if (diveAttack)
            {
                transform.position += moveDir * moveSpeed * diveSpeedMultiplier * Time.deltaTime;
            }
            else
            {
                transform.position += moveDir * moveSpeed * Time.deltaTime;
            }
        }
        else
        {
            // ground enemy logic giữ nguyên
            transform.position +=
                new Vector3(Mathf.Sign(directionX) * moveSpeed * Time.deltaTime, 0, 0);
        }
    }

    private void OnDisable()
    {
        if (HasParameter("moving"))
            anim.SetBool("moving", false);
    }

    public void SetFrozen(bool frozen)
    {
        isFrozen = frozen;
        if (HasParameter("moving"))
            anim.SetBool("moving", !frozen);
    }

    private bool HasParameter(string paramName)
    {
        foreach (AnimatorControllerParameter param in anim.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }
}
