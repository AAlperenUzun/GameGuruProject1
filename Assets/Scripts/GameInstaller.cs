using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<GameController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<GridPresenter>().FromComponentInHierarchy().AsSingle();
        Container.Bind<UIContoller>().FromComponentInHierarchy().AsSingle();
        Container.Bind<BoardController>().AsTransient();
        Container.Bind<InputController>().AsTransient();

    }
}