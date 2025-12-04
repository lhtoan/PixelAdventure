using UnityEngine;
using Unity.Cinemachine;

public class AutoRoomZoom : MonoBehaviour
{
    [SerializeField] private Collider2D roomBounds;
    [SerializeField] private CinemachineCamera roomCam;
    [SerializeField] private RoomCameraController controller;  // 🔥 thêm dòng này
    [SerializeField] private bool enableDebug = true;

    private void Start()
    {
        FitCameraToRoom();
    }

    public void FitCameraToRoom()
    {
        if (roomBounds == null || roomCam == null)
        {
            return;
        }

        Bounds b = roomBounds.bounds;

        float roomWidth = b.size.x;
        float roomHeight = b.size.y;

        // Aspect ratio cho camera ortho
        float aspect = (float)Screen.width / Screen.height;

        float sizeBasedOnWidth = roomWidth / (2f * aspect);
        float sizeBasedOnHeight = roomHeight / 2f;

        float finalSize = Mathf.Max(sizeBasedOnWidth, sizeBasedOnHeight);

        // ⭐ Set zoom vào RoomCam
        roomCam.Lens.OrthographicSize = finalSize;

        // ⭐ Gửi lens sang controller để sử dụng khi active room
        if (controller != null)
            controller.SetRoomLens(finalSize);


        // DebugLog(
        //     "=== AutoRoomZoom Debug ===\n" +
        //     $"📌 RoomBounds: {roomBounds.name}\n" +
        //     $"➡ Width: {roomWidth:F2}, Height: {roomHeight:F2}\n" +
        //     $"📺 Aspect: {aspect:F2}\n" +
        //     $"🔹 sizeBasedOnWidth = {sizeBasedOnWidth:F2}\n" +
        //     $"🔹 sizeBasedOnHeight = {sizeBasedOnHeight:F2}\n" +
        //     $"✔ FINAL SIZE = {finalSize:F2}\n"
        // );
    }

    private void DebugLog(string msg)
    {
        if (enableDebug)
            Debug.Log(msg, this);
    }
}
