namespace ECS.Components.Game
{
    using UnityEngine;

    public struct MoveComponent
    {
        public Vector2 Direction;
        public float Speed;
        public Transform Target;
    }
}

namespace ECS.Authoring.Helpers
{
    using Components.Game;
    using Game.CharacterSystem.Data;
    using Leopotam.EcsProto;
    using UnityEngine;

    public static partial class CreateComponentHelper
    {
        public static ref MoveComponent CreateMove(
            this ProtoEntity entity,
            ProtoPool<MoveComponent> pool,
            Transform target,
            CharacterData data)
        {
            ref MoveComponent moveComponent = ref pool.Add(entity);
            moveComponent.Target = target;
            moveComponent.Speed = data.MovementData.MoveSpeed;

            return ref moveComponent;
        }
    }
}