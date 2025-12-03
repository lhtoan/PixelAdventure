// using UnityEngine;

// public class FlyingEnemy : MonoBehaviour
// {
//     [SerializeField] public float speed = 2f;
//     [SerializeField] public bool chase = false;
//     [SerializeField] public Transform startingPoint;
//     private GameObject player;

//     void Start()
//     {
//         player=GameObject.FindGameObjectWithTag("Player");

//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if(player==null) return;
//         if (chase == true)
//         {
//             Chase();
//         }
//         else
//         {
//             ReturnStartPoint();
//         }
//         FlipEnemy();
//     }

//     private void Chase()
//     {
//         transform.position=Vector2.MoveTowards(transform.position, player.transform.position, speed*Time.deltaTime);
//     }

//     private void ReturnStartPoint()
//     {
//         transform.position = Vector2.MoveTowards(transform.position, startingPoint.transform.position, speed * Time.deltaTime);
//     }

//     private void FlipEnemy()
//     {
//         if (transform.position.x > player.transform.position.x)
//         {
//             transform.rotation=Quaternion.Euler(0,0,0);
//         } else
//         {
//             transform.rotation = Quaternion.Euler(0, 180, 0);
//         }
//     }
// }
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    [SerializeField] public float speed = 2f;
    [SerializeField] public bool chase = false;
    [SerializeField] public Transform startingPoint;

    private GameObject player;
    private Vector3 originalPos;   // Lưu vị trí ban đầu

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        // Lưu lại vị trí ban đầu của StartPoint
        originalPos = startingPoint.position;
    }

    void Update()
    {
        if (player == null) return;

        if (chase)
        {
            Chase();
        }
        else
        {
            ReturnStartPoint();
        }

        FlipEnemy();
    }

    private void Chase()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            player.transform.position,
            speed * Time.deltaTime
        );
    }

    private void ReturnStartPoint()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            originalPos,     // ← dùng vị trí gốc, không dùng transform của StartPoint nữa
            speed * Time.deltaTime
        );
    }

    private void FlipEnemy()
    {
        if (!chase) return;

        Vector3 scale = transform.localScale;

        // Nếu enemy mặc định nhìn TRÁI
        if (transform.position.x > player.transform.position.x)
            scale.x = Mathf.Abs(scale.x);   // nhìn trái
        else
            scale.x = -Mathf.Abs(scale.x);  // nhìn phải

        transform.localScale = scale;
    }

}
