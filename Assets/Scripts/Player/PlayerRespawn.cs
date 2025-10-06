using UnityEngine;
using UnityEngine.SceneManagement; 

public class PlayerRespawn : MonoBehaviour
{
    private Transform currentCheckpoint;
    private Health playerHealth;

    [SerializeField] private int maxLives = 3;
    private int currentLives;

    private void Awake()
    {
        playerHealth = GetComponent<Health>();
        currentLives = maxLives;
    }

    public void PlayerDied()
    {
        currentLives--;

        if (currentLives > 0)
        {
            Respawn();
        }
        else
        {
            // GameOver();
        }
    }

    private void Respawn()
    {
        playerHealth.Respawn(); // hồi máu
        transform.position = currentCheckpoint != null
            ? currentCheckpoint.position
            : transform.position; // nếu chưa chạm checkpoint thì ở nguyên chỗ cũ
    }

    // private void GameOver()
    // {
    //     Debug.Log("Game Over! Hết mạng.");
    //     //load màn hình Game Over, ...
    //     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    // }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CheckPoint"))
        {
            currentCheckpoint = collision.transform;
            collision.GetComponent<Collider2D>().enabled = false; // tắt checkpoint sau khi chạm
        }
    }
}
