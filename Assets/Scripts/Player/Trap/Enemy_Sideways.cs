using UnityEngine;

public class Enemy_Sideways : MonoBehaviour
{
    // [SerializeField] private float movementDistance;
    // [SerializeField] private float speed;
    // [SerializeField] private float damage = 1f;
    // private bool movingLeft;
    // private float leftEdge;
    // private float rightEdge;

    // private void Awake()
    // {
    //     leftEdge = transform.position.x - movementDistance;
    //     rightEdge = transform.position.x + movementDistance;
    // }

    // private void Update()
    // {
    //     if (movingLeft)
    //     {
    //         if (transform.position.x > leftEdge)
    //         {
    //             transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, transform.position.z);
    //         }
    //         else
    //             movingLeft = false;
    //     }
    //     else
    //     {
    //         if (transform.position.x < rightEdge)
    //         {
    //             transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
    //         }
    //         else
    //             movingLeft = true;
    //     }
    // }

    // private void OnTriggerEnter2D(Collider2D collision)
    // {
    //     if (collision.tag == "Player")
    //     {
    //         collision.GetComponent<Health>().TakeDamage(damage);
    //     }
    // }

    [Header("Platform Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    private Vector2 targetPosition;

    [Header("Damage Settings")]
    [SerializeField] private float damage = 1f; // Lượng sát thương gây ra khi chạm vào

    private void Start()
    {
        targetPosition = startPoint.position;
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, startPoint.position) < 0.1f)
        {
            targetPosition = endPoint.position;
        }

        if (Vector2.Distance(transform.position, endPoint.position) < 0.1f)
        {
            targetPosition = startPoint.position;
        }

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);
        }
    }


}
