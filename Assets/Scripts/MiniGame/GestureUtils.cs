using System.Collections.Generic;
using UnityEngine;

public static class GestureUtils
{
    // Resample a polyline to targetCount points (uses linear interpolation)
    public static List<Vector2> Resample(List<Vector2> pts, int targetCount)
    {
        if (pts == null || pts.Count == 0) return new List<Vector2>();

        // if already small, just duplicate last point to reach count
        if (pts.Count == 1)
        {
            var single = new List<Vector2>();
            for (int i = 0; i < targetCount; i++) single.Add(pts[0]);
            return single;
        }

        float totalLen = PathLength(pts);
        float interval = totalLen / (targetCount - 1);

        List<Vector2> newPts = new List<Vector2> { pts[0] };
        float D = 0f;

        for (int i = 1; i < pts.Count; i++)
        {
            Vector2 a = pts[i - 1];
            Vector2 b = pts[i];
            float d = Vector2.Distance(a, b);

            if (d == 0f) continue;

            while (D + d >= interval)
            {
                float t = (interval - D) / d;
                Vector2 next = Vector2.Lerp(a, b, t);
                newPts.Add(next);

                // move a to next, reduce remaining segment
                a = next;
                d = Vector2.Distance(a, b);
                D = 0f;
            }

            D += d;
        }

        // if missing last points, append last original
        while (newPts.Count < targetCount)
            newPts.Add(pts[pts.Count - 1]);

        return newPts;
    }

    static float PathLength(List<Vector2> pts)
    {
        float sum = 0f;
        for (int i = 1; i < pts.Count; i++)
            sum += Vector2.Distance(pts[i - 1], pts[i]);
        return sum;
    }

    // Normalize: translate min -> 0, scale so max dimension = 1 (keeps aspect)
    public static List<Vector2> Normalize(List<Vector2> pts)
    {
        if (pts == null || pts.Count == 0) return new List<Vector2>();

        float minX = float.MaxValue, minY = float.MaxValue;
        float maxX = float.MinValue, maxY = float.MinValue;
        foreach (var p in pts)
        {
            if (p.x < minX) minX = p.x;
            if (p.y < minY) minY = p.y;
            if (p.x > maxX) maxX = p.x;
            if (p.y > maxY) maxY = p.y;
        }

        float width = maxX - minX;
        float height = maxY - minY;
        float scale = Mathf.Max(width, height);
        if (scale <= 0f) scale = 1f;

        List<Vector2> outPts = new List<Vector2>(pts.Count);
        for (int i = 0; i < pts.Count; i++)
        {
            float nx = (pts[i].x - minX) / scale;
            float ny = (pts[i].y - minY) / scale;
            outPts.Add(new Vector2(nx, ny));
        }

        return outPts;
    }

    // Convenience: convert List<Vector3> (world) to List<Vector2> (local UI positions)
    public static List<Vector2> ToVector2(List<Vector3> pts)
    {
        var outPts = new List<Vector2>(pts.Count);
        foreach (var p in pts) outPts.Add(new Vector2(p.x, p.y));
        return outPts;
    }
}
