using UnityEngine;

public class UniversalEnemyMovement : MonoBehaviour, IEnemyMovement
{
    [Header("Target Settings")]
    public Transform target;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public bool isFlying = false;

    [Header("Flip Settings")]
    public bool flipInverted = false;    // ⭐ ONLY mushroom bật cái này

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
        float direction = target.position.x - transform.position.x;

        // ⭐ Flip đúng hướng (hoặc ngược nếu flipInverted)
        if (direction != 0)
        {
            float flipDir = Mathf.Sign(direction);

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

        if (isFlying)
        {
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }
        else
        {
            transform.position +=
                new Vector3(Mathf.Sign(direction) * moveSpeed * Time.deltaTime, 0, 0);
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
