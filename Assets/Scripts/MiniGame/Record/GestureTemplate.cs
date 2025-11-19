using System;
using UnityEngine;

[Serializable]
public class GesturePoint { public float x, y; }

[Serializable]
public class GestureTemplate
{
    public string name;
    public GesturePoint[] points;
}
