namespace Core
{
    using ECS.Aspects;
    using ECS.Extensions;
    using ECS.Factories;
    using ECS.Services;
    using ECS.Systems.Game;
    using Game.CharacterSystem.Data;
    using Game.Data;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using Leopotam.EcsProto.Unity;
    using UnityEngine;
    using UtilsModule.Execute;
    using UtilsModule.Execute.Interfaces;
    using UtilsModule.Execute.Mode;
    using UtilsModule.Other;

    public class GameInstaller : MonoBehaviour, IExecuteHolder
    {
        private ProtoWorld _mainWorld;
        private IProtoSystems _mainSystems;

        public ExecuteMethod Method => ExecuteMethod.Awake;
        public int Priority { get; set; }

        public Executor GetExecutor()
        {
            return new ExecutorSync(Init);
        }

        private void Init()
        {
            InitMainWorld();
        }

        private void InitMainWorld()
        {
            GameAspect gameAspect = new GameAspect();
            _mainWorld = new ProtoWorld(gameAspect);

            _mainSystems = new ProtoSystems(_mainWorld);

            _mainSystems
                .AddModule(new UnityModule())
                .AddModule(new AutoInjectModule());

            AddDependencies();
            AddEvents();
            AddSystems();

            _mainSystems.Init();

            PlayerFactory.Create(gameAspect, _mainSystems.Service<CharacterDataHolder>().GetData(UnitType.Player));

            return;

            void AddDependencies()
            {
                var gameConfig = DI.Resolve<GameConfig>();

                _mainSystems
                    .AddService(gameConfig)
                    .AddService(DI.Resolve<GameInput>())
                    .AddService(gameConfig.PrefabsHolder)
                    .AddService(gameConfig.CharacterDataHolder)
                    .AddService(gameConfig.HealthBarDataHolder)
                    .AddService(new CharacterPoolService())
                    .InitHere<CharacterPoolService>();
            }

            void AddEvents()
            {
                _mainSystems
                    .AddService(new DeadEventHandler());
            }

            void AddSystems()
            {
                _mainSystems
                    .AddSystem(new FactorySystem())
                    .AddSystem(new PlayerInputSystem())
                    .AddSystem(new MoveSystem())
                    .AddSystem(new EventHandleSystem())
                    .AddSystem(new EventResetSystem())
                    .AddSystem(new TimerSystem())
                    .AddSystem(new HealthViewSystem())
                    .AddSystem(new DisposeSystem());
            }
        }

        private void Update()
        {
            _mainSystems?.Run();
        }

        private void OnDestroy()
        {
            _mainSystems?.Destroy();
            _mainSystems = null;

            _mainWorld?.Destroy();
            _mainWorld = null;
        }
    }
}