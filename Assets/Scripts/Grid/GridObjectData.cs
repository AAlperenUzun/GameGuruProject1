using System;
using UnityEngine;
using UnityEngine.Serialization;

public readonly struct GridObjectData
{
    public readonly  Vector2Int Position;
    public readonly GridObjectType TypeContainer;
    

    public GridObjectData(Vector2Int position, GridObjectType objectType)
    {
        Position = position;
        TypeContainer = objectType;
    }
}

[Serializable]
public struct GridObjectTypeContainer : IEquatable<GridObjectTypeContainer>
{
    public GridObjectType GridObjectType;

    public GridObjectTypeContainer(GridObjectType gridObjectType)
    {
        GridObjectType = gridObjectType;
    }

    public static bool operator ==(GridObjectTypeContainer lhs, GridObjectTypeContainer rhs)
    {
        return lhs.Equals(rhs);
    }

    public static bool operator !=(GridObjectTypeContainer lhs, GridObjectTypeContainer rhs)
    {
        return !lhs.Equals(rhs);
    }

    public bool Equals(GridObjectTypeContainer other)
    {
        return GridObjectType == other.GridObjectType;
    }

    public override bool Equals(object obj)
    {
        return obj is GridObjectTypeContainer other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)GridObjectType, (int)GridObjectType);
    }
}
