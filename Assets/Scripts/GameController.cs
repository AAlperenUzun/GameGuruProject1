using System.Collections;
using UnityEngine;
using Zenject;

public class GameController: MonoBehaviour
{
    
    [SerializeField] private Vector2Int _gridSize;
    public Vector2Int GridSize => _gridSize;
    [Inject] private GridPresenter _gridPresenter;

    public GameObject gridPrefab;
    public GameObject xPrefab;
    [Inject]public InputController InputController { get; private set; }
    [Inject]public BoardController BoardController { get; private set; }
    protected void Awake()
    {
        Initialize();
    }

    public void GenerateAgain(Vector2Int gridSize)
    {
        _gridSize = gridSize;
        Dispose();
        Initialize();
    }
    public void Initialize()
    {
        InitializeEssentials();
    }

    public void Dispose()
    {
        BoardController.Dispose();
    }
    

    private void InitializeEssentials()
    {
        _gridPresenter.Initialize();
        BoardController.Generate();
    }
}