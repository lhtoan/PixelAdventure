using UnityEngine;
using UnityEditor;
using System;

public class CheckpointID : MonoBehaviour
{
    public string checkpointID; // auto-generated

    private void Reset()
    {
        GenerateID();
    }

    private void OnValidate()
    {
        // Nếu ID rỗng → tạo mới
        if (string.IsNullOrEmpty(checkpointID))
            GenerateID();
    }

    private void GenerateID()
    {
        checkpointID = Guid.NewGuid().ToString(); // tạo ID duy nhất
#if UNITY_EDITOR
        EditorUtility.SetDirty(this); // lưu thay đổi vào scene
#endif
    }
}
