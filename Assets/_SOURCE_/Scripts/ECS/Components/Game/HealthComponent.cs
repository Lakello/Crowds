namespace ECS.Components.Game
{
    public struct HealthComponent
    {
        public float MaxHealth;
        public float CurrentHealth;
        public float HeightWorldOffset;

        public float NormalizedHealth => CurrentHealth / MaxHealth;
    }
}

namespace ECS.Authoring.Helpers
{
    using Components.Game;
    using Game.CharacterSystem.Data;
    using Leopotam.EcsProto;

    public static partial class CreateComponentHelper
    {
        public static ref HealthComponent CreateHealth(this ProtoEntity entity, ProtoPool<HealthComponent> pool, CharacterData data)
        {
            ref HealthComponent healthComponent = ref pool.Add(entity);
            healthComponent.MaxHealth = data.HealthData.MaxHealth;
            healthComponent.CurrentHealth = data.HealthData.MaxHealth;
            healthComponent.HeightWorldOffset = data.HealthData.HeightWorldOffset;
            
            return ref healthComponent;
        }
    }
}