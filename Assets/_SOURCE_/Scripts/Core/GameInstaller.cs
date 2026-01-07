namespace _SOURCE_.Scripts.Core
{
    using ECS.Aspects;
    using ECS.Components;
    using ECS.Data.Services;
    using ECS.Factories;
    using ECS.Systems;
    using Game.Data;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.Unity;
    using UnityEngine;
    using UtilsModule.Execute;
    using UtilsModule.Execute.Interfaces;
    using UtilsModule.Other;

    public class GameInstaller : MonoBehaviour, IExecuteHolder
    {
        private ProtoWorld _world;
        private IProtoSystems _systems;

        public ExecuteMethod Method => ExecuteMethod.Awake;
        public int Priority { get; set; }

        public Executor GetExecutor()
        {
            return new ExecutorSync(Init);
        }

        private void Init()
        {
            GameAspect gameAspect = new GameAspect();
            _world = new ProtoWorld(gameAspect);

            _systems = new ProtoSystems(_world);

            _systems
                .AddModule(new UnityModule())
                
                .AddService(new DataHolderService<PrefabsHolder>
                {
                    Data = DI.Resolve<PrefabsHolder>(),
                })
                .AddService(new DataHolderService<MovementDataHolder>
                {
                    Data = DI.Resolve<MovementDataHolder>(),
                }, typeof(MovementDataHolder))
                
                .AddSystem(new PlayerInputSystem())
                .AddSystem(new MoveSystem())
                
                .AddSystem(new EventHandleSystem())
                .AddSystem(new ResetEventsSystem())
                
                .Init();

            ref EventComponent createPlayerEvent = ref gameAspect.EventsPool.NewEntity(out _);
            createPlayerEvent.IsActive = true;
            createPlayerEvent.Handler = new PlayerFactory();
        }

        private void Update()
        {
            _systems?.Run();
        }

        private void OnDestroy()
        {
            _systems?.Destroy();
            _systems = null;

            _world?.Destroy();
            _world = null;
        }
    }
}