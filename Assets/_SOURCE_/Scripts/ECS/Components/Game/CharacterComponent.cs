namespace ECS.Components.Game
{
    using UnityEngine;

    public struct CharacterComponent
    {
        public Vector3 Position;
    }
}

namespace ECS.Authoring.Helpers
{
    using Components.Game;
    using Game.CharacterSystem;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;

    public static partial class CreateComponentHelper
    {
        public static ref CharacterComponent CreateCharacter(
            this ProtoEntity entity,
            ProtoPool<CharacterComponent> pool, 
            Character character)
        {
            ref CharacterComponent characterComponent = ref pool.Add(entity);

            pool.World().PackEntity(entity).Register(character);
            
            return ref characterComponent;
        }
    }
}