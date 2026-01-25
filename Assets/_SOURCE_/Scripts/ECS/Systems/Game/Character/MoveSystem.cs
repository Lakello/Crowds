namespace ECS.Systems.Game
{
    using Aspects;
    using Components.Game;
    using global::Game.CharacterSystem.Data;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using UnityEngine;

    public class MoveSystem : IProtoRunSystem
    {
        [DI] private CharacterDataHolder _characterDataHolder;
        [DI] private GameAspect _gameAspect;

        [DI] private ProtoIt _moveIt = new ProtoIt(It.Inc<MoveComponent>());

        public void Run()
        {
            foreach (ProtoEntity e in _moveIt)
            {
                ref MoveComponent moveComponent = ref _gameAspect.MovePool.Get(e);

                if (moveComponent.Direction == Vector2.zero)
                {
                    continue;
                }

                float moveSpeed = moveComponent.Speed;

                Vector3 offset = new Vector3(
                    moveComponent.Direction.x * moveSpeed * Time.deltaTime,
                    0,
                    moveComponent.Direction.y * moveSpeed * Time.deltaTime);

                moveComponent.Target.position += offset;
            }
        }
    }
}