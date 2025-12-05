// using Unity.Cinemachine;
// using UnityEngine;
// using UnityEngine.Rendering.Universal;

// public class CameraMissionController : MonoBehaviour
// {
//     [Header("References")]
//     public CinemachineCamera vcam;
//     public PixelPerfectCamera pixelPerfect;

//     [Header("Default Settings")]
//     public float defaultLensSize = 5f;
//     public int defaultPPU = 18;

//     [Header("Mission Settings")]
//     public float missionLensSize = 10f;
//     public int missionPPU = 9;

//     public void ApplyMissionCamera()
//     {
//         if (vcam != null)
//             vcam.Lens.OrthographicSize = missionLensSize;

//         if (pixelPerfect != null)
//             pixelPerfect.assetsPPU = missionPPU;
//     }

//     public void ApplyDefaultCamera()
//     {
//         if (vcam != null)
//             vcam.Lens.OrthographicSize = defaultLensSize;

//         if (pixelPerfect != null)
//             pixelPerfect.assetsPPU = defaultPPU;
//     }

// }
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

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

    [Header("Transition")]
    public float transitionDuration = 1f;   // ⭐ Thời gian chuyển mượt

    private Coroutine transitionRoutine;

    public void ApplyMissionCamera()
    {
        StartTransition(missionLensSize, missionPPU);
    }

    public void ApplyDefaultCamera()
    {
        StartTransition(defaultLensSize, defaultPPU);
    }

    private void StartTransition(float targetSize, int targetPPU)
    {
        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        transitionRoutine = StartCoroutine(TransitionCamera(targetSize, targetPPU));
    }

    private IEnumerator TransitionCamera(float targetSize, int targetPPU)
    {
        float startSize = vcam.Lens.OrthographicSize;
        int startPPU = pixelPerfect.assetsPPU;

        float t = 0f;

        while (t < transitionDuration)
        {
            t += Time.unscaledDeltaTime;  // ⭐ Để pause game vẫn hoạt động mượt
            float lerp = t / transitionDuration;

            // Lens mượt
            vcam.Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, lerp);

            // PPU mượt
            pixelPerfect.assetsPPU = Mathf.RoundToInt(Mathf.Lerp(startPPU, targetPPU, lerp));

            yield return null;
        }

        // Đảm bảo set đúng giá trị cuối
        vcam.Lens.OrthographicSize = targetSize;
        pixelPerfect.assetsPPU = targetPPU;
    }
}
