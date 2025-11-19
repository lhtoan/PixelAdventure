using System.Collections.Generic;
using UnityEngine;

public class GestureRecorder : MonoBehaviour
{
    public GestureTemplateSO template;   // mẫu nào đang được ghi
    public bool isRecording = false;

    public void BeginRecording()
    {
        isRecording = true;
        Debug.Log("[Recorder] Recording ON");
    }

    public void Capture(List<Vector2> pts)
    {
        if (!isRecording) return;

        template.normalizedPoints = new List<Vector2>(pts);
        Debug.Log("[Recorder] Saved template!");

        isRecording = false;

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(template);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }
}

