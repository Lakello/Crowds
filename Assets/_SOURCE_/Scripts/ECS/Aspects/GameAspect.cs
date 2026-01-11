namespace ECS.Aspects
{
    using Authoring;
    using Components.Game;
    using Components.Game.Markers;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;

    public class GameAspect : ProtoAspectInject
    {
        public ProtoPool<FactoryComponent> FactoryPool;
        public ProtoPool<TimerComponent> TimerPool;
        public ProtoPool<EntityHolderComponent> SpawnedEntityHolderPool;

        public ProtoPool<AIMarker> AiPool;
        
        public ProtoPool<CharacterComponent> CharacterPool;
        public ProtoPool<PlayerMarker> PlayerPool;

        public ProtoPool<HealthComponent> HealthPool;

        public ProtoPool<MoveComponent> MovePool;

        public ProtoPool<EventHandlerComponent> EventsPool;
    }
}