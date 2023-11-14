using UnityEngine;
using Zenject;

public class BaseGridObjectController
{
    public GridObjectData Data { get; }
    
    private GameController gameController;
    public void Init(GameController gameController)
    {
        this.gameController = gameController;
    }
    public BaseGridObjectController(GridObjectData data)
    {
        Data = data;
    }

    public bool Interact()
    {
        if (Data.TypeContainer==GridObjectType.X)
        {
            return gameController.BoardController.TryDestroyConnectedObjectsAtPosition(Data.Position);
        }
        else
        {
            return gameController.BoardController.TryAddX(Data.Position, Data);
        }
  
    }

    public Vector2Int GetPosition() => Data.Position;
    
    public GridObjectData GetPresenterData()
    {
        return new GridObjectData(Data.Position, Data.TypeContainer);
    }
    
}