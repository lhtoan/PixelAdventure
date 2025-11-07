using UnityEngine;

public class Projecttile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float maxDistance = 3;
    [SerializeField] private float fireDamage = 5; // 🔥 Damage cho Fire
    [SerializeField] private float iceDamage = 5; // ❄️ Damage cho Ice
    [SerializeField] private float defaultDamage = 1f; // ⚪ Dự phòng

    private Vector3 startPosition;
    private float direction;
    private bool hit;
    private float lifetime;

    private BoxCollider2D boxCollider;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (hit) return;

        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(movementSpeed, 0, 0);

        lifetime += Time.deltaTime;

        if (Vector3.Distance(transform.position, startPosition) > maxDistance)
        {
            gameObject.SetActive(false);
            return;
        }

        if (lifetime > 5) gameObject.SetActive(false);
    }

    public void SetDirection(float _direction, float angle = 0f)
    {
        lifetime = 0;
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        startPosition = transform.position;

        float localScaleX = transform.localScale.x;
        if (Mathf.Sign(localScaleX) != _direction)
            localScaleX = -localScaleX;

        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
        transform.rotation = Quaternion.Euler(0, 0, angle * -_direction);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        boxCollider.enabled = false;
        if (anim != null)
            anim.SetTrigger("explode");

        if (collision.CompareTag("Enemy"))
        {
            float damageToApply = defaultDamage;

            // 💥 Xác định damage theo hệ
            if (CompareTag("Fire"))
                damageToApply = fireDamage;
            else if (CompareTag("Ice"))
                damageToApply = iceDamage;

            collision.GetComponent<Health>()?.TakeDamage(damageToApply);

            // ❄️ Hiệu ứng Ice
            if (CompareTag("Ice"))
            {
                var freeze = collision.GetComponent<FreezeEnemy>();
                freeze?.TriggerIceHit();
            }
            // 🔥 Hiệu ứng Fire
            else if (CompareTag("Fire"))
            {
                var burn = collision.GetComponent<BurnEnemy>();
                burn?.TriggerBurn();
            }
        }
        else if (collision.CompareTag("Box"))
        {
            collision.GetComponent<BreakableBox>()?.TakeDamage(1);
        }
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