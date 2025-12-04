using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Rendering.Universal;

public class RoomCameraController : MonoBehaviour
{
    [Header("Camera References")]
    public CinemachineCamera followCam;
    public CinemachineCamera roomCam;
    public PixelPerfectCamera pixelPerfect;

    [Header("Default Settings")]
    public float defaultLensSize = 5f;
    public int defaultPPU = 18;

    private float roomLensSize;

    public void SetRoomLens(float newSize)
    {
        roomLensSize = newSize;
    }

    public void ActivateRoomCamera()
    {

        // Switch priority
        roomCam.Priority.Value = 20;
        followCam.Priority.Value = 0;

        // Apply zoom room
        roomCam.Lens.OrthographicSize = roomLensSize;

        // ❗ Tắt Pixel Perfect → cho phép zoom mượt
        if (pixelPerfect != null)
            pixelPerfect.enabled = false;

    }

    public void DeactivateRoomCamera()
    {
        

        // Switch back
        followCam.Priority.Value = 20;
        roomCam.Priority.Value = 0;

        // Restore gameplay camera zoom
        followCam.Lens.OrthographicSize = defaultLensSize;

        // ❗ Bật lại Pixel Perfect
        if (pixelPerfect != null)
        {
            pixelPerfect.enabled = true;
            pixelPerfect.assetsPPU = defaultPPU;
        }

    }
}
