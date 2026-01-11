namespace ECS.Systems.Game
{
    using Aspects;
    using Components.Game;
    using Components.Game.Markers;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;

    public class AIInputSystem : IProtoRunSystem
    {
        [DI] private GameAspect _gameAspect;
        
        [DI] private ProtoIt _aiIt = new ProtoIt(new []
        {
            typeof(AIMarker),
            typeof(MoveComponent),
        });
        
        public void Run()
        {
            foreach (ProtoEntity entity in _aiIt)
            {
                ref var moveComponent = ref _gameAspect.MovePool.Get(entity);
            }
        }
    }
}