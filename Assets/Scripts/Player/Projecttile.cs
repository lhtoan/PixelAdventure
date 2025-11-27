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

    // ⭐ Lưu enemy đã bị hit
    private HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();

    private void OnEnable()
    {
        // ⭐ Luôn gán lại để tránh NullReference
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();
        if (anim == null) anim = GetComponent<Animator>();

        hitEnemies.Clear();
        lifetime = 0;
        startPosition = transform.position;

        // ⭐ Luôn bật collider khi spawn
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

        // ⭐ Luôn bật object đây là đúng chỗ
        gameObject.SetActive(true);

        // ⭐ Gán collider lại nếu bị null
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider2D>();

        boxCollider.enabled = true;

        // ⭐ Hướng
        float localScaleX = transform.localScale.x;
        if (Mathf.Sign(localScaleX) != _direction)
            localScaleX = -localScaleX;

        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
        transform.rotation = Quaternion.Euler(0, 0, angle * -_direction);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ⭐ Enemy → chỉ hit 1 lần
        if (collision.CompareTag("Enemy"))
        {
            boxCollider.enabled = false;
            anim?.SetTrigger("explode");
            if (!hitEnemies.Contains(collision))
            {
                hitEnemies.Add(collision);

                // ⭐ Chặn collider của enemy này với projectile vĩnh viễn
                Collider2D enemyCol = collision.GetComponent<Collider2D>();
                if (enemyCol != null)
                    Physics2D.IgnoreCollision(boxCollider, enemyCol, true);

                float damageToApply = defaultDamage;

                if (CompareTag("Fire")) damageToApply = fireDamage;
                else if (CompareTag("Ice")) damageToApply = iceDamage;

                collision.GetComponent<Health>()?.TakeDamage(damageToApply);

                if (CompareTag("Ice"))
                    collision.GetComponent<FreezeEnemy>()?.TriggerIceHit();

                if (CompareTag("Fire"))
                    collision.GetComponent<BurnEnemy>()?.TriggerBurn();
            }

            return; // xuyên qua enemy
        }

        // ⭐ Wall hoặc Box → explode
        if (collision.CompareTag("Box"))
        {
            boxCollider.enabled = false;
            anim?.SetTrigger("explode");

            if (collision.CompareTag("Box"))
                collision.GetComponent<BreakableBox>()?.TakeDamage(1);
        }
    }


    // ⭐ Animation event gọi hàm này
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}



// private void OnTriggerEnter2D(Collider2D collision)
// {
//     hit = true;
//     boxCollider.enabled = false;
//     anim.SetTrigger("explode");

//     if (collision.CompareTag("Enemies"))
//     {
//         collision.GetComponent<EnemyHealth>()?.TakeDamage(1f);
//     }
//     else if (collision.CompareTag("Enemy"))
//     {
//         collision.GetComponent<Health>().TakeDamage(1);
//     }
//     else if (collision.CompareTag("Box"))
//     {
//         collision.GetComponent<BreakableBox>()?.TakeDamage(1);
//     }
// }

// public void SetDirection(float _direction)
// {
//     lifetime = 0;
//     direction = _direction;
//     gameObject.SetActive(true);
//     hit = false;
//     boxCollider.enabled = true;

//     startPosition = transform.position;

//     float localScaleX = transform.localScale.x;
//     if (Mathf.Sign(localScaleX) != _direction)
//         localScaleX = -localScaleX;

//     transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
// }

// public void SetDirection(float _direction, float angle = 0f)
// {
//     lifetime = 0;
//     direction = _direction;
//     gameObject.SetActive(true);
//     hit = false;
//     boxCollider.enabled = true;

//     startPosition = transform.position;

//     float localScaleX = transform.localScale.x;
//     if (Mathf.Sign(localScaleX) != _direction)
//         localScaleX = -localScaleX;

//     transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);

//     // Thêm hướng xoay để bắn lên hoặc xuống
//     transform.rotation = Quaternion.Euler(0, 0, angle * -_direction);
// }

// private void OnTriggerEnter2D(Collider2D collision)
// {
//     hit = true;
//     boxCollider.enabled = false;
//     anim.SetTrigger("explode");

//     if (collision.CompareTag("Enemy"))
//     {
//         collision.GetComponent<Health>()?.TakeDamage(1);
//     }
//     else if (collision.CompareTag("Box"))
//     {
//         collision.GetComponent<BreakableBox>()?.TakeDamage(1);
//     }

//     // ❄️ Nếu là Iceball -> đóng băng enemy
//     if (CompareTag("Ice"))
//     {
//         FreezeEnemy freeze = collision.GetComponent<FreezeEnemy>();
//         if (freeze != null)
//         {
//             freeze.TriggerFreeze(5f); // đóng băng 1 giây
//         }
//     }
// }