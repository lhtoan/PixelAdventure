using UnityEngine;
using System.Collections.Generic;

public class Projecttile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float maxDistance = 3;
    [SerializeField] private float fireDamage = 5;
    [SerializeField] private float iceDamage = 5;
    [SerializeField] private float defaultDamage = 1f;

    private Vector3 startPosition;
    private float direction;
    private float lifetime;

    private BoxCollider2D boxCollider;
    private Animator anim;

    private HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();

    private PlayerAttack attacker;
    public void SetAttacker(PlayerAttack a)
    {
        attacker = a;
    }

    private void OnEnable()
    {
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();
        if (anim == null) anim = GetComponent<Animator>();

        hitEnemies.Clear();
        lifetime = 0;
        startPosition = transform.position;
        boxCollider.enabled = true;
    }

    private void Update()
    {
        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(movementSpeed, 0, 0);

        lifetime += Time.deltaTime;

        if (Vector3.Distance(transform.position, startPosition) > maxDistance)
        {
            gameObject.SetActive(false);
            return;
        }

        if (lifetime > 5)
            gameObject.SetActive(false);
    }

    public void SetDirection(float _direction, float angle = 0f)
    {
        direction = _direction;

        gameObject.SetActive(true);

        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider2D>();

        boxCollider.enabled = true;

        float localScaleX = transform.localScale.x;
        if (Mathf.Sign(localScaleX) != _direction)
            localScaleX = -localScaleX;

        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
        transform.rotation = Quaternion.Euler(0, 0, angle * -_direction);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            boxCollider.enabled = false;
            anim?.SetTrigger("explode");

            if (!hitEnemies.Contains(collision))
            {
                hitEnemies.Add(collision);

                Collider2D enemyCol = collision.GetComponent<Collider2D>();
                if (enemyCol != null)
                    Physics2D.IgnoreCollision(boxCollider, enemyCol, true);

                // ===============================
                //   FIRE DAMAGE
                // ===============================
                if (CompareTag("Fire"))
                {
                    collision.GetComponent<Health>()?.TakeDamage(fireDamage);
                    collision.GetComponent<BurnEnemy>()?.TriggerBurn(attacker);
                }

                // ===============================
                //   ICE DAMAGE (NEW FULL SYSTEM)
                // ===============================
                else if (CompareTag("Ice"))
                {
                    FreezeEnemy freeze = collision.GetComponent<FreezeEnemy>();
                    if (freeze != null)
                    {
                        // ⭐ Gọi hàm mới để gây damage + tích dame + tăng stack + nổ nếu đủ
                        freeze.ApplyIceDamage(iceDamage, attacker);
                    }
                }

                // ===============================
                //   DEFAULT DAMAGE
                // ===============================
                else
                {
                    collision.GetComponent<Health>()?.TakeDamage(defaultDamage);
                }
            }

            return;
        }

        if (collision.CompareTag("Box"))
        {
            boxCollider.enabled = false;
            anim?.SetTrigger("explode");

            collision.GetComponent<BreakableBox>()?.TakeDamage(1);
        }
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}



// using UnityEngine;
// using System.Collections.Generic;

// public class Projecttile : MonoBehaviour
// {
//     [SerializeField] private float speed;
//     [SerializeField] private float maxDistance = 3;
//     [SerializeField] private float fireDamage = 5;
//     [SerializeField] private float iceDamage = 5;
//     [SerializeField] private float defaultDamage = 1f;

//     private Vector3 startPosition;
//     private float direction;
//     private float lifetime;

//     private BoxCollider2D boxCollider;
//     private Animator anim;

//     private HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();

//     private PlayerAttack attacker;
//     public void SetAttacker(PlayerAttack a)
//     {
//         attacker = a;
//     }


//     private void OnEnable()
//     {
//         if (boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();
//         if (anim == null) anim = GetComponent<Animator>();

//         hitEnemies.Clear();
//         lifetime = 0;
//         startPosition = transform.position;
//         boxCollider.enabled = true;
//     }

//     private void Update()
//     {
//         float movementSpeed = speed * Time.deltaTime * direction;
//         transform.Translate(movementSpeed, 0, 0);

//         lifetime += Time.deltaTime;

//         if (Vector3.Distance(transform.position, startPosition) > maxDistance)
//         {
//             gameObject.SetActive(false);
//             return;
//         }

//         if (lifetime > 5)
//             gameObject.SetActive(false);
//     }

//     public void SetDirection(float _direction, float angle = 0f)
//     {
//         direction = _direction;

//         gameObject.SetActive(true);

//         if (boxCollider == null)
//             boxCollider = GetComponent<BoxCollider2D>();

//         boxCollider.enabled = true;

//         float localScaleX = transform.localScale.x;
//         if (Mathf.Sign(localScaleX) != _direction)
//             localScaleX = -localScaleX;

//         transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
//         transform.rotation = Quaternion.Euler(0, 0, angle * -_direction);
//     }

//     private void OnTriggerEnter2D(Collider2D collision)
//     {
//         if (collision.CompareTag("Enemy"))
//         {
//             boxCollider.enabled = false;
//             anim?.SetTrigger("explode");

//             if (!hitEnemies.Contains(collision))
//             {
//                 hitEnemies.Add(collision);

//                 Collider2D enemyCol = collision.GetComponent<Collider2D>();
//                 if (enemyCol != null)
//                     Physics2D.IgnoreCollision(boxCollider, enemyCol, true);

//                 float damageToApply = defaultDamage;

//                 if (CompareTag("Fire")) damageToApply = fireDamage;
//                 else if (CompareTag("Ice")) damageToApply = iceDamage;

//                 collision.GetComponent<Health>()?.TakeDamage(damageToApply);

//                 if (CompareTag("Ice"))
//                     collision.GetComponent<FreezeEnemy>()?.TriggerIceHit();

//                 if (CompareTag("Fire"))
//                 {
//                     collision.GetComponent<BurnEnemy>()?.TriggerBurn(attacker);
//                 }

//             }

//             return;
//         }

//         if (collision.CompareTag("Box"))
//         {
//             boxCollider.enabled = false;
//             anim?.SetTrigger("explode");

//             collision.GetComponent<BreakableBox>()?.TakeDamage(1);
//         }
//     }

//     private void Deactivate()
//     {
//         gameObject.SetActive(false);
//     }
// }
