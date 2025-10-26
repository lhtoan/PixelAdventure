using UnityEngine;

public class Projecttile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float maxDistance = 3;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        boxCollider.enabled = false;
        anim.SetTrigger("explode");

        if (collision.CompareTag("Enemies"))
        {
            collision.GetComponent<EnemyHealth>()?.TakeDamage(1f);
        }
        else if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Health>().TakeDamage(1);
        }
        else if (collision.CompareTag("Box"))
        {
            collision.GetComponent<BreakableBox>()?.TakeDamage(1);
        }
    }

    public void SetDirection(float _direction)
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
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
