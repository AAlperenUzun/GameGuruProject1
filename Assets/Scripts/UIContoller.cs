using TMPro;
using UnityEngine;
using Zenject;

public class UIContoller : MonoBehaviour
{
    [Inject] private GameController gameController;
    public TMP_InputField textX;
    public TMP_InputField textY;
    public void Generate()
    {
        Vector2Int newGrid = new Vector2Int(int.Parse(textX.text), int.Parse(textY.text));
        gameController.GenerateAgain(newGrid);
    }
}
