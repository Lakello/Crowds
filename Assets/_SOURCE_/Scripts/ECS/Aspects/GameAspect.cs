namespace _SOURCE_.Scripts.ECS.Aspects
{
    using Components;
    using Components.Marker;
    using Components.Markers;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;

    public class GameAspect : ProtoAspectInject
    {
        public ProtoPool<CharacterComponent> CharacterPool;
        public ProtoPool<Player> PlayerPool;
        
        public ProtoPool<MoveComponent> MovePool;
        
        public ProtoPool<EventComponent> EventsPool;
    }
}