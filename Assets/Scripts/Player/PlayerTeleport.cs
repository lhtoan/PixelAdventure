// using UnityEngine;

// public class PlayerTeleport : MonoBehaviour
// {
//     private GameObject currentTeleporter;

//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.E))
//         {
//             if (currentTeleporter != null)
//             {
//                 transform.position = currentTeleporter.GetComponent<Teleporter>().GetDestination().position;
//             }
//         }
//     }

//     private void OnTriggerEnter2D(Collider2D collision)
//     {
//         if (collision.CompareTag("Teleporter"))
//         {
//             currentTeleporter = collision.gameObject;
//         }
//     }

//     private void OnTriggerExit2D(Collider2D collision)
//     {
//         if (collision.CompareTag("Teleporter"))
//         {
//             if (collision.gameObject == currentTeleporter)
//             {
//                 currentTeleporter = null;
//             }
//         }
//     }
// }
using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    private bool canTeleport = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canTeleport) return;

        if (collision.CompareTag("Teleporter"))
        {
            Teleporter tp = collision.GetComponent<Teleporter>();
            if (tp != null)
            {
                StartCoroutine(Teleport(tp));
            }
        }
    }

    private System.Collections.IEnumerator Teleport(Teleporter tp)
    {
        canTeleport = false;

        transform.position = tp.GetDestination().position;

        // chống loop teleporter
        yield return new WaitForSeconds(1f);

        canTeleport = true;
    }
}
