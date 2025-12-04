using UnityEngine;

public class IceSpikeDamage : MonoBehaviour
{
    [SerializeField] private float damage = 0.5f;
    [SerializeField] private LayerMask playerLayer;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (((1 << col.gameObject.layer) & playerLayer) != 0)
        {
            col.GetComponent<Health>()?.TakeDamage(damage);
        }
    }

    public void SetDamage(float d)
    {
        damage = d;
    }
}
