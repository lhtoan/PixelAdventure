using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            Destroy(collision.gameObject);
            gameManager.AddScore(1);
            // Debug.Log("Coin");
        }
        else if (collision.CompareTag("Trap"))
        {
            GetComponent<Health>().TakeDamage(1);
        }
        else if (collision.CompareTag("Heart"))
        {
            GetComponent<Health>().AddHealth(1);
            Destroy(collision.gameObject);
        }
    }
}
