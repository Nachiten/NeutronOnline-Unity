using System;
using UnityEngine;

[Serializable]
public struct GridVisualTypeColor
{
    public GridVisualType gridVisualType;
    public Color color;
}

public enum GridVisualType
{
    White,
    Blue,
    Red,
    RedSoft,
    Yellow
}
