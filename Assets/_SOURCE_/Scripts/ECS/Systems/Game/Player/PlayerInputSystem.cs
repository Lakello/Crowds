namespace ECS.Systems.Game
{
    using Aspects;
    using Components.Game;
    using Components.Game.Markers;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using UnityEngine;

    public class PlayerInputSystem : IProtoInitSystem, IProtoRunSystem, IProtoDestroySystem
    {
        [DI] private GameInput _input;
        [DI] private GameAspect _gameAspect;

        [DI] private ProtoIt _moveIt = new ProtoIt(It.Inc<PlayerMarker, MoveComponent>());

        public void Init(IProtoSystems systems)
        {
            _input.Enable();
        }

        public void Run()
        {
            foreach (ProtoEntity e in _moveIt)
            {
                ref MoveComponent moveComponent = ref _gameAspect.MovePool.Get(e);

                moveComponent.Direction = _input.Character.MoveX.ReadValue<Vector2>();
            }
        }

        public void Destroy()
        {
            _input.Disable();
        }
    }
}