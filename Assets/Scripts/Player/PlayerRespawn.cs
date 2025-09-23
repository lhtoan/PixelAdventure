using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Transform currentCheckPoint;
    private Health playerHealth;

    private void Awake()
    {
        playerHealth = GetComponent<Health>();
    }

    public void Respawn()
    {
        transform.position = currentCheckPoint.position;
    }
}
