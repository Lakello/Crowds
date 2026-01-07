namespace _SOURCE_.Scripts.ECS.Systems
{
    using Aspects;
    using Components;
    using Leopotam.EcsProto;

    public class EventHandleSystem : IProtoInitSystem, IProtoRunSystem
    {
        private ProtoIt _eventsIt;
        private GameAspect _gameAspect;
        
        public void Init(IProtoSystems systems)
        {
            ProtoWorld world = systems.World();

            _gameAspect = (GameAspect)world.Aspect(typeof(GameAspect));

            _eventsIt = new ProtoIt(new[]
            {
                typeof(EventComponent),
            });
            
            _eventsIt.Init(world);
        }

        public void Run()
        {
            foreach (var e in _eventsIt)
            {
                ref var eventComponent = ref _gameAspect.EventsPool.Get(e);

                if (eventComponent.IsActive)
                {
                    eventComponent.Handler.HandleEvent();
                }
            }
        }
    }
}