using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Grid
{
    private readonly byte _rows;
    private readonly byte _columns;
    private readonly List<Vector2Int> _connectedPoints = new List<Vector2Int>();

    public BaseGridObjectController[] GridObjects { get; private set; }

    public int TopX { get; }
    public int TopY { get; }

    public Grid(byte rows, byte columns)
    {
        _rows = rows;
        _columns = columns;
        TopY = rows - 1;
        TopX = columns - 1;

        GridObjects = new BaseGridObjectController[columns * rows];
    }

    public bool TryGetGridObject(int x, int y, out BaseGridObjectController data)
    {
        data = null;

        if (x >= _columns || x < 0)
            return false;

        if (y >= _rows || y < 0)
            return false;

        data = GridObjects[ToSingleDimensionIndex(x, y)];
        return true;
    }

    public void SetGridObject(BaseGridObjectController controller, int x, int y)
    {
        int index = ToSingleDimensionIndex(x, y);

        if (index >= GridObjects.Length || index < 0)
            throw new Exception("Object is out of bounds");

        GridObjects[index] = controller;
    }

    public void SetGridObject(BaseGridObjectController controller)
    {
        SetGridObject(controller, controller.GetPosition().x, controller.GetPosition().y);
    }
    
    public bool TryGetConnectedPositions(Vector2Int origin, out List<Vector2Int> connectedPoints)
    {
        _connectedPoints.Clear();
        int index = ToSingleDimensionIndex(origin.x, origin.y);

        NativeArray<GridObjectData> grid = new NativeArray<GridObjectData>(GridObjects.Length, Allocator.TempJob);
        NativeArray<bool> visited = new NativeArray<bool>(grid.Length, Allocator.TempJob);
        NativeArray<bool> connected = new NativeArray<bool>(grid.Length, Allocator.TempJob);
        NativeQueue<int> queue = new NativeQueue<int>(Allocator.TempJob);

        for (int i = 0; i < GridObjects.Length; i++)
        {
            grid[i] = GridObjects[i].GetPresenterData();
        }

        visited[index] = true;
        connected[index] = true;
        queue.Enqueue(index);

        FindConnectedObjectsJob job = new FindConnectedObjectsJob()
        {
            Grid = grid,
            Rows = _rows,
            Columns = _columns,
            Visited = visited,
            IsConnected = connected,
            Queue = queue
        };

        JobHandle jobHandle = job.Schedule();
        jobHandle.Complete();


        for (int i = 0; i < connected.Length; i++)
        {
            if (connected[i])
            {
                _connectedPoints.Add(ToTwoDimensionIndex(i));
            }
        }

        connectedPoints = _connectedPoints;

        grid.Dispose();
        visited.Dispose();
        connected.Dispose();
        queue.Dispose();

        return connectedPoints.Count > 2;
    }

    public IEnumerable<(BaseGridObjectController, Vector2Int)> GetUpdatedPositions()
    {
        for (var i = 0; i < GridObjects.Length; i++)
        {
            if (GridObjects[i] == null)
                continue;

            Vector2Int correctPosition = ToTwoDimensionIndex(i);

            if (correctPosition != GridObjects[i].GetPosition())
            {
                yield return (GridObjects[i], correctPosition);
            }
        }
    }

    public int GetAvailableEmptyYPosition(int x)
    {
        int row = TopY;

        for (int y = TopY; y >= 0; y--)
        {
            if (GridObjects[ToSingleDimensionIndex(x, y)] == null)
            {
                row = y;
            }
        }

        return row;
    }

    public int ToSingleDimensionIndex(int x, int y)
    {
        return x + y * _columns;
    }

    public Vector2Int ToTwoDimensionIndex(int index)
    {
        return new Vector2Int(index % _columns, index / _columns);
    }
}

[BurstCompile(CompileSynchronously = true, FloatPrecision = FloatPrecision.Low, FloatMode = FloatMode.Fast,
    OptimizeFor = OptimizeFor.Performance, DisableSafetyChecks = false)]
public struct FindConnectedObjectsJob : IJob
{
    [ReadOnly] public NativeArray<GridObjectData> Grid;
    [ReadOnly] public byte Rows;
    [ReadOnly] public byte Columns;

    public NativeArray<bool> Visited;
    public NativeArray<bool> IsConnected;
    public NativeQueue<int> Queue;

    public void Execute()
    {
        while (Queue.Count > 0)
        {
            int current = Queue.Dequeue();
            int currentRow = current / Columns;
            int currentCol = current % Columns;

            for (int i = 0; i < GameUtility.DirectionCount; i++)
            {
                int newRow = currentRow + GameUtility.GetRowDirection(i);
                int newCol = currentCol + GameUtility.GetColDirection(i);
                int index = newRow * Columns + newCol;

                if (newRow >= 0 && newRow < Rows && newCol >= 0 && newCol < Columns &&
                    !Visited[index])
                {
                    Visited[index] = true;

                    if (GameUtility.IsMatching(Grid[current], Grid[index]))
                    {
                        Queue.Enqueue(index);
                        IsConnected[index] = true;
                    }
                }
            }
        }
    }
}