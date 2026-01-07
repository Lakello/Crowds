namespace _SOURCE_.Scripts.ECS.Systems
{
    using Aspects;
    using Components;
    using Leopotam.EcsProto;
    using UnityEngine;

    public class PlayerInputSystem : IProtoInitSystem, IProtoRunSystem
    {
        private GameInput _input;
        private ProtoIt _moveIt;
        private GameAspect _gameAspect;
        
        public void Init(IProtoSystems systems)
        {
            _input = new GameInput();
            _input.Enable();
            
            ProtoWorld world = systems.World();

            _gameAspect = (GameAspect)world.Aspect(typeof(GameAspect));

            _moveIt = new ProtoIt(new[]
            {
                typeof(MoveComponent),
            });
            
            _moveIt.Init(world);
        }

        public void Run()
        {
            foreach (ProtoEntity e in _moveIt)
            {
                ref MoveComponent moveComponent = ref _gameAspect.MovePool.Get(e);

                moveComponent.Input = _input.Character.MoveX.ReadValue<Vector2>();
            }
        }
    }
}