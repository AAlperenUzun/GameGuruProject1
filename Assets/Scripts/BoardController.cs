using System;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;
using Zenject;

public class BoardController
{

    private readonly InputController _inputController;

    public Grid Grid { get; private set; }
    
    private readonly Dictionary<Vector2Int, GameObject> _presenters=new Dictionary<Vector2Int, GameObject>();

    private GameController gameController;

    [Inject]
    public void Init(GameController gameController)
    {
        this.gameController = gameController;
    }

    public BoardController(InputController inputController)
    {
        _inputController = inputController;
        
        Initialize();
    }

    public void Initialize()
    {
        RegisterListeners();
    }

    public void Dispose()
    {
        // UnregisterListeners();
        foreach (var presenter in _presenters.Values)
        {
            LeanPool.Despawn(presenter);
        }
        _presenters.Clear();
    }
    
    public void Generate()
    {
        Grid = new Grid((byte)gameController.GridSize.y, (byte)gameController.GridSize.x);

        for (var i = 0; i < Grid.GridObjects.Length; i++)
        {
            Vector2Int position = Grid.ToTwoDimensionIndex(i);
            GridObjectData data = new GridObjectData(position, GridObjectType.Empty);
            CreateGridObject(data);
        }
    }

    private void CreateGridObject(GridObjectData objectData)
    {
        BaseGridObjectController controller = CreateGridPrefab(objectData, false);
        AddGridObject(controller);
    }

    private void AddGridObject(BaseGridObjectController controller)
    {
        Grid.SetGridObject(controller);
    }
    
    public bool TryAddX(Vector2Int position, GridObjectData objectData)
    {
        DestroyPrefab(position);
        
        GridObjectTypeContainer startObjectType = new GridObjectTypeContainer();
        startObjectType.GridObjectType = GridObjectType.X;
        var typeContainer = startObjectType;
        objectData = new GridObjectData(position, typeContainer.GridObjectType);
        if (!Grid.TryGetGridObject(position.x, position.y, out BaseGridObjectController controller))
        {
            return false;
        }

        var xController = CreateGridPrefab(objectData, true);
        Grid.SetGridObject(null, position.x, position.y);
        controller.Init(gameController);
        AddGridObject(xController);
        OnClicked(new Vector3(position.x, position.y, 0));
        return true;
    }

    public bool TryDestroyConnectedObjectsAtPosition(Vector2Int target)
    {
        if (!Grid.TryGetConnectedPositions(target, out List<Vector2Int> connectedPositions))
            return false;

        foreach (Vector2Int position in connectedPositions)
        {
            Grid.TryGetGridObject(position.x, position.y, out BaseGridObjectController controller);
            Grid.SetGridObject(null, position.x, position.y);
        }
        CreateAndFallRoutine(connectedPositions);
        return true;
    }

    private void DestroyPrefab(Vector2Int target)
    {
        _presenters.TryGetValue(target, out GameObject removeObject);
        _presenters.Remove(target);
        LeanPool.Despawn(removeObject);
    }
    private void CreateAndFallRoutine(List<Vector2Int> positions)
    {
        foreach (Vector2Int position in positions)
        {
            DestroyPrefab(position);
            
            int y = Grid.GetAvailableEmptyYPosition(position.x);
            Vector2Int newPosition = new Vector2Int(position.x, position.y);
            Vector2Int startPosition = newPosition;
            
            GridObjectData data = new GridObjectData(startPosition, GridObjectType.Empty);

            BaseGridObjectController cubeController = CreateGridPrefab(data, false);

            Grid.SetGridObject(cubeController, newPosition.x, newPosition.y);
        }
    }

    private BaseGridObjectController CreateGridPrefab(GridObjectData data, bool isX)
    {
        GameObject presenter = null;
        if (isX)
        {
            presenter = LeanPool.Spawn(gameController.xPrefab);
        }
        else
        {
            presenter = LeanPool.Spawn(gameController.gridPrefab);
        }
        presenter.transform.position=new Vector3(data.Position.x, data.Position.y);
        _presenters.Add(data.Position, presenter);
        var newController = new BaseGridObjectController(data);
        newController.Init(gameController);
        return newController;
    }

    private void RegisterListeners()
    {
        _inputController.Clicked += OnClicked;
    }

    private void UnregisterListeners()
    {
        _inputController.Clicked -= OnClicked;
    }

    private void OnClicked(Vector3 point)
    {
        Vector2Int gridPosition = new Vector2Int(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y));

        if (Grid.TryGetGridObject(gridPosition.x, gridPosition.y, out BaseGridObjectController controller))
        {
            bool successful = controller.Interact();
            
        }
    }
}