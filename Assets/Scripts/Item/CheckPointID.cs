using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CheckpointID : MonoBehaviour
{
    public string checkpointID;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Không chạy trong PlayMode
        if (Application.isPlaying)
            return;

        // Nếu là prefab gốc → KHÔNG tạo ID
        if (PrefabUtility.IsPartOfPrefabAsset(gameObject))
            return;

        // Nếu object là instance trong scene nhưng ID rỗng → tạo ID mới
        if (string.IsNullOrEmpty(checkpointID))
        {
            GenerateID();
        }
    }
#endif

    private void GenerateID()
    {
        checkpointID = Guid.NewGuid().ToString();

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
}
