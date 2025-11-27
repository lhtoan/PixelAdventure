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
        float distance = Vector2.Distance(transform.position, target.position);
        float verticalDistance = Mathf.Abs(transform.position.y - target.position.y);

        // ⭐ Ground enemy: không đuổi nếu khác tầng
        if (!isFlying)
        {
            if (verticalDistance > 1.5f)
            {
                if (HasParameter("moving"))
                    anim.SetBool("moving", false);
                return;
            }
        }


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

        // float distance = Vector2.Distance(transform.position, target.position);

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
