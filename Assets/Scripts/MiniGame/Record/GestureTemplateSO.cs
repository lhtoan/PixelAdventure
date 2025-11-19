using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GestureTemplate", menuName = "Minigame/Gesture Template")]
public class GestureTemplateSO : ScriptableObject
{
    public DrawSymbol symbol;
    public List<Vector2> normalizedPoints;
}
