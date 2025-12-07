// using UnityEngine;

// public class ChaseControll : MonoBehaviour
// {
//     public FlyingEnemy[] enemyArray;

//     private void OnTriggerEnter2D(Collider2D collision)
//     {
//         if (collision.CompareTag("Player"))
//         {
//             foreach(FlyingEnemy enemy in enemyArray)
//             {
//                 enemy.chase = true;
//             }
//         }      
//     }

//     private void OnTriggerExit2D(Collider2D collision)
//     {
//         if (collision.CompareTag("Player"))
//         {
//             foreach (FlyingEnemy enemy in enemyArray)
//             {
//                 enemy.chase = false;
//             }
//         }
//     }
// }
using UnityEngine;

public class ChaseControll : MonoBehaviour
{
    [Header("Enemies To Control")]
    public FlyingEnemy[] enemyArray;

    [Header("Camera Control")]
    public bool enableCameraSwitch = false;   // ⭐ Tuỳ chọn bật/tắt chuyển camera
    public CameraMissionController cameraControl;

    private bool cameraActivated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // ⭐ Bật chế độ đuổi
        foreach (FlyingEnemy enemy in enemyArray)
        {
            if (enemy != null)
                enemy.chase = true;
        }

        // ⭐ Chuyển camera nếu tuỳ chọn bật
        if (enableCameraSwitch && cameraControl != null && !cameraActivated)
        {
            cameraActivated = true;
            cameraControl.ApplyMissionCamera();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // ⭐ Tắt chase khi player rời vùng
        foreach (FlyingEnemy enemy in enemyArray)
        {
            if (enemy != null)
                enemy.chase = false;
        }

        // ⭐ Trả camera về mặc định nếu tuỳ chọn bật
        if (enableCameraSwitch && cameraControl != null && cameraActivated)
        {
            cameraActivated = false;
            cameraControl.ApplyDefaultCamera();
        }
    }
}
