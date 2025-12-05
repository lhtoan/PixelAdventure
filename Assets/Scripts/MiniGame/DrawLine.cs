// using System.Collections.Generic;
// using UnityEngine;

// public class DrawLine : MonoBehaviour
// {
//     public LineRenderer lineRenderer;
//     public RectTransform drawArea;
//     public Camera uiCamera;

//     List<Vector3> points = new List<Vector3>();
//     bool isDrawing = false;

//     void Update()
//     {
//         bool inside = RectTransformUtility.RectangleContainsScreenPoint(
//             drawArea,
//             Input.mousePosition,
//             uiCamera
//         );

//         if (inside)
//         {
//             if (Input.GetMouseButtonDown(0))
//                 StartDrawing();

//             if (Input.GetMouseButton(0) && isDrawing)
//                 AddPoint();

//             if (Input.GetMouseButtonUp(0) && isDrawing)
//                 StopDrawing();
//         }
//         else
//         {
//             if (Input.GetMouseButtonUp(0))
//                 StopDrawing();
//         }
//     }

//     void StartDrawing()
//     {
//         isDrawing = true;
//         points.Clear();
//         lineRenderer.positionCount = 0;
//     }

//     void AddPoint()
//     {
//         RectTransformUtility.ScreenPointToLocalPointInRectangle(
//             drawArea,
//             Input.mousePosition,
//             uiCamera,
//             out Vector2 localPos
//         );

//         // Convert UI local → world
//         Vector3 worldPos = drawArea.TransformPoint(localPos);

//         // FIX: đảm bảo lineRenderer luôn nằm cùng Z-plane
//         worldPos.z = 0f;

//         points.Add(worldPos);

//         lineRenderer.positionCount = points.Count;
//         lineRenderer.SetPosition(points.Count - 1, worldPos);
//     }

//     void StopDrawing()
//     {
//         isDrawing = false;
//     }

//     public void ResetLine()
//     {
//         points.Clear();
//         lineRenderer.positionCount = 0;
//     }
// }
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public LineRenderer lineRenderer;
    public RectTransform drawArea;
    public Camera uiCamera;
    public DrawMinigame manager; // <-- gán DrawMinigame ở Inspector (recommended)

    List<Vector3> points = new List<Vector3>();
    bool isDrawing = false;

    void Awake()
    {
        // ⭐ Đảm bảo nét vẽ luôn nằm TRÊN UI
        if (lineRenderer != null)
        {
            lineRenderer.sortingLayerName = "MiniGame";
            lineRenderer.sortingOrder = 50;
        }
    }

    void Update()
    {
        bool inside = RectTransformUtility.RectangleContainsScreenPoint(
            drawArea,
            Input.mousePosition,
            uiCamera
        );

        if (inside)
        {
            if (Input.GetMouseButtonDown(0))
                StartDrawing();

            if (Input.GetMouseButton(0) && isDrawing)
                AddPoint();

            if (Input.GetMouseButtonUp(0) && isDrawing)
                StopDrawing();
        }
        else
        {
            if (Input.GetMouseButtonUp(0) && isDrawing)
                StopDrawing();
        }
    }

    // void StartDrawing()
    // {
    //     isDrawing = true;
    //     points.Clear();
    //     if (lineRenderer != null) lineRenderer.positionCount = 0;
    // }

    void StartDrawing()
    {
        // ⭐ Reset nét cũ khi BẮT ĐẦU VẼ NÉT MỚI
        ResetLine();

        isDrawing = true;
        points.Clear();
    }


    void AddPoint()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            drawArea,
            Input.mousePosition,
            uiCamera,
            out Vector2 localPos
        );

        Vector3 worldPos = drawArea.TransformPoint(localPos);
        worldPos.z = 0f;

        // avoid adding points too close
        if (points.Count > 0 && Vector3.Distance(points[points.Count - 1], worldPos) < 0.02f)
            return;

        points.Add(worldPos);

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPosition(points.Count - 1, worldPos);
        }
    }

    void StopDrawing()
    {
        isDrawing = false;

        // convert world points -> drawArea local 2D points
        List<Vector2> localPts = new List<Vector2>(points.Count);
        foreach (var w in points)
        {
            Vector3 local3 = drawArea.InverseTransformPoint(w);
            localPts.Add(new Vector2(local3.x, local3.y));
        }

        // resample + normalize (you already have GestureUtils in project)
        var res = GestureUtils.Resample(localPts, 32);
        var norm = GestureUtils.Normalize(res);

        Debug.Log("[DrawLine] StopDrawing. rawPoints = " + points.Count + " => resampled = " + res.Count);

        // send to manager (use inspector-assigned manager)
        if (manager != null)
        {
            manager.OnPlayerDrawFinished(norm);
        }
        else
        {
            Debug.LogWarning("[DrawLine] manager (DrawMinigame) is NULL - assign in Inspector or call programmatically.");
        }
    }

    public void ResetLine()
    {
        points.Clear();
        if (lineRenderer != null) lineRenderer.positionCount = 0;
    }
}
