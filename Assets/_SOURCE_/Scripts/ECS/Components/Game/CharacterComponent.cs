namespace ECS.Components.Game
{
    using global::Game.CharacterSystem;

    public struct CharacterComponent
    {
        public Character Character;
    }
}

namespace ECS.Authoring.Helpers
{
    using Components.Game;
    using Game.CharacterSystem;
    using Leopotam.EcsProto;

    public static partial class CreateComponentHelper
    {
        public static ref CharacterComponent CreateCharacter(
            this ProtoEntity entity,
            ProtoPool<CharacterComponent> pool, 
            Character character)
        {
            ref CharacterComponent characterComponent = ref pool.Add(entity);
            characterComponent.Character = character;
            
            return ref characterComponent;
        }
    }
}