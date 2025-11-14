using UnityEngine;

public class UniversalEnemyMovement : MonoBehaviour, IEnemyMovement
{
    [Header("Target Settings")]
    public Transform target;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public bool isFlying = false;

    [Header("Follow Range")]
    public float followRange = 5f;    // Enemy chỉ follow trong phạm vi này
    public float stopDistance = 0.5f; // Enemy đứng lại khi quá gần

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
            anim.SetBool("moving", false);
            return;
        }

        if (target == null)
        {
            anim.SetBool("moving", false);
            return;
        }

        float distance = Vector2.Distance(transform.position, target.position);

        // ❌ Nếu target quá xa → không follow
        if (distance > followRange)
        {
            anim.SetBool("moving", false);
            return;
        }

        // ❌ Nếu quá gần → đứng lại
        if (distance < stopDistance)
        {
            anim.SetBool("moving", false);
            return;
        }

        MoveToTarget();
    }

    private void MoveToTarget()
    {
        float direction = target.position.x - transform.position.x;

        // Flip theo hướng
        if (direction != 0)
            enemyModel.localScale = new Vector3(Mathf.Abs(initScale.x) * Mathf.Sign(direction),
                                                initScale.y, initScale.z);

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
        anim.SetBool("moving", false);
    }


    public void SetFrozen(bool frozen)
    {
        isFrozen = frozen;
        anim.SetBool("moving", !frozen);
    }
}
