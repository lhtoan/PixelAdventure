using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Platform Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private Vector2 targetPosition;

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

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(this.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Transform player = collision.transform;
            Invoke(nameof(DetachPlayer), 0.02f);
        }
    }

    private void DetachPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            player.transform.SetParent(null);
    }


}
