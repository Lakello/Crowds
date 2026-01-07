namespace _SOURCE_.Scripts.ECS.Aspects
{
    using Components;
    using Components.Marker;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;

    public class GameAspect : ProtoAspectInject
    {
        public ProtoPool<CharacterComponent> CharacterPool;
        public ProtoPool<EventComponent> EventsPool;
    }
}