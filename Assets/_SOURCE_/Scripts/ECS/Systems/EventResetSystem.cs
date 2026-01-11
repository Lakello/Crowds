namespace ECS.Systems.Game
{
    using Aspects;
    using Components.Game;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;

    public class EventResetSystem : IProtoRunSystem
    {
        [DI] private GameAspect _gameAspect;

        [DI] private ProtoIt _eventsIt = new ProtoIt(new[]
        {
            typeof(EventHandlerComponent),
        });

        public void Run()
        {
            foreach (var e in _eventsIt)
            {
                ref var eventComponent = ref _gameAspect.EventsPool.Get(e);
                eventComponent.IsActive = false;
            }
        }
    }
}