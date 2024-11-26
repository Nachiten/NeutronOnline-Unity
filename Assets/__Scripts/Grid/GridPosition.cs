using System;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public struct GridPosition : IEquatable<GridPosition>, INetworkSerializable
{
    public int x;
    public int y;

    public GridPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    
    public GridPosition(Vector2 position)
    {
        x = (int) position.x;
        y = (int) position.y;
    }
    
    public override string ToString()
    {
        return $"({x}, {y})";
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref x);
        serializer.SerializeValue(ref y);
    }

    public static bool operator ==(GridPosition a, GridPosition b)
    {
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(GridPosition a, GridPosition b)
    {
        return !(a == b);
    }

    public bool Equals(GridPosition other)
    {
        return this == other;
    }

    public override bool Equals(object obj)
    {
        return obj is GridPosition other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y);
    }

    public static GridPosition operator +(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x + b.x, a.y + b.y);
    }

    public static GridPosition operator -(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x - b.x, a.y - b.y);
    }
    
    public static GridPosition Null => new(int.MinValue, int.MinValue);
    
    public static bool operator true(GridPosition gridPosition)
    {
        return gridPosition != Null;
    }
    
    public static bool operator false(GridPosition gridPosition)
    {
        return gridPosition == Null;
    }
    
    public static bool operator !(GridPosition gridPosition)
    {
        return gridPosition == Null;
    }
}