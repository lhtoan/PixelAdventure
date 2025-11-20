using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraMissionController : MonoBehaviour
{
    [Header("References")]
    public CinemachineCamera vcam;
    public PixelPerfectCamera pixelPerfect;

    [Header("Default Settings")]
    public float defaultLensSize = 5f;
    public int defaultPPU = 18;

    [Header("Mission Settings")]
    public float missionLensSize = 10f;
    public int missionPPU = 9;

    public void ApplyMissionCamera()
    {
        if (vcam != null)
            vcam.Lens.OrthographicSize = missionLensSize;

        if (pixelPerfect != null)
            pixelPerfect.assetsPPU = missionPPU;
    }

    public void ApplyDefaultCamera()
    {
        if (vcam != null)
            vcam.Lens.OrthographicSize = defaultLensSize;

        if (pixelPerfect != null)
            pixelPerfect.assetsPPU = defaultPPU;
    }
}
