using UnityEngine;

public static class GameUtility
{
    public const int DirectionCount = 4;
    private static readonly int[] _rowDirections = { -1, 0, 0, 1 };
    private static readonly int[] _colDirections = { 0, -1, 1, 0 };
    
    public static int GetRowDirection(int index) => _rowDirections[index];
    public static int GetColDirection(int index) => _colDirections[index];

    public static bool IsMatching(GridObjectData source, GridObjectData target)
    {
        if (source.TypeContainer == GridObjectType.Empty)
            return false;

        if (source.TypeContainer == GridObjectType.X &&
            target.TypeContainer == GridObjectType.X)
            return true;

        return false;
    }
}