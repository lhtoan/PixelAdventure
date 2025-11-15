using UnityEngine;
using System.Collections;

public class ExplosionTriggerWithDeath : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float delayBeforeExplode = 5f;
    public float explosionDamage = 20f;
    public float explosionRadius = 1.5f;
    public LayerMask targetLayer;

    [Header("Animation")]
    public Animator anim;
    public string explodeTrigger = "explode";

    private bool isCounting = false;
    private bool exploded = false;

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (exploded) return;

        if (((1 << coll.gameObject.layer) & targetLayer) != 0)
        {
            if (!isCounting)
                StartCoroutine(StartExplosionCountdown());
        }
    }

    private void OnTriggerExit2D(Collider2D coll)
    {
        if (exploded) return;

        if (((1 << coll.gameObject.layer) & targetLayer) != 0)
        {
            isCounting = false;
            StopAllCoroutines();
        }
    }

    IEnumerator StartExplosionCountdown()
    {
        isCounting = true;

        yield return new WaitForSeconds(delayBeforeExplode);

        if (isCounting && !exploded)
        {
            TriggerExplosion();
        }
    }

    void TriggerExplosion()
    {
        exploded = true;
        isCounting = false;

        if (anim != null)
        {
            anim.SetTrigger(explodeTrigger);
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, targetLayer);

        foreach (var hit in hits)
        {
            Health hp = hit.GetComponent<Health>();
            if (hp != null)
                hp.TakeDamage(explosionDamage);
        }
    }

    // Called from explosion animation event
    public void OnExplosionFinished()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
