using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static readonly List<Vector2> directions = new(
        new Vector2[] {
            new(-1, -1),
            new(0, -1),
            new(1, -1),
            new(-1, 0),
            new(1, 0),
            new(-1, 1),
            new(0, 1),
            new(1, 1) }
        );
}
