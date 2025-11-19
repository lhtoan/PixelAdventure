using System.Collections.Generic;
using UnityEngine;

public static class GestureRecognizer
{
    // Dynamic Time Warping distance
    public static float DTW(List<Vector2> a, List<Vector2> b)
    {
        int n = a.Count;
        int m = b.Count;

        float[,] dp = new float[n + 1, m + 1];

        const float INF = 999999f;

        for (int i = 0; i <= n; i++)
            for (int j = 0; j <= m; j++)
                dp[i, j] = INF;

        dp[0, 0] = 0f;

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                float cost = Vector2.Distance(a[i - 1], b[j - 1]);
                dp[i, j] = cost + Mathf.Min(dp[i - 1, j],        // delete
                                            dp[i, j - 1],        // insert
                                            dp[i - 1, j - 1]);   // match
            }
        }

        return dp[n, m];
    }

    // Compare player drawing to all templates
    public static DrawSymbol Recognize(List<Vector2> player, List<GestureTemplateSO> templates)
    {
        float bestScore = 999999f;
        DrawSymbol bestSymbol = DrawSymbol.LineVertical; // default

        foreach (var t in templates)
        {
            if (t.normalizedPoints == null || t.normalizedPoints.Count == 0)
                continue;

            float d = DTW(player, t.normalizedPoints);

            if (d < bestScore)
            {
                bestScore = d;
                bestSymbol = t.symbol;
            }
        }

        Debug.Log($"[GestureRecognizer] Best = {bestSymbol}, score = {bestScore}");
        return bestSymbol;
    }
}
