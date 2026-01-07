namespace _SOURCE_.Scripts.ECS.Systems
{
    using System;
    using Aspects;
    using Components;
    using Data.Services;
    using Game.Data;
    using Leopotam.EcsProto;
    using UnityEngine;

    public class MoveSystem : IProtoInitSystem, IProtoRunSystem
    {
        private MovementDataHolder _movementDataHolder;
        
        private ProtoIt _moveIt;
        private GameAspect _gameAspect;
        
        public void Init(IProtoSystems systems)
        {
            ProtoWorld world = systems.World();

            _gameAspect = (GameAspect)world.Aspect(typeof(GameAspect));

            _moveIt = new ProtoIt(new[]
            {
                typeof(MoveComponent),
            });
            
            _moveIt.Init(world);
            
            _movementDataHolder = (systems.Services()[typeof(MovementDataHolder)] as DataHolderService<MovementDataHolder>)?.Data;
            if (_movementDataHolder == null)
            {
                throw new NullReferenceException("_movementDataHolder");
            }
        }

        public void Run()
        {
            foreach (ProtoEntity e in _moveIt)
            {
                ref MoveComponent moveComponent = ref _gameAspect.MovePool.Get(e);

                MovementData data = _movementDataHolder.GetData(moveComponent.MovementType);
                float moveSpeed = data.MoveSpeed;
                
                Vector3 direction = new Vector3(
                    moveComponent.Input.x * moveSpeed * Time.deltaTime,
                    0,
                    moveComponent.Input.y * moveSpeed * Time.deltaTime);
                
                moveComponent.Target.position += direction;
            }
        }
    }
}