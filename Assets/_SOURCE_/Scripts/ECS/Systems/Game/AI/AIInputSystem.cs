namespace ECS.Systems.Game
{
    using Aspects;
    using Components.Game;
    using Components.Game.Markers;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using UnityEngine;

    public class AIInputSystem : IProtoRunSystem
    {
        [DI] private GameAspect _gameAspect;
        
        [DI] private readonly ProtoIt _aiIt = new ProtoIt(It.Inc<AIMarker, MoveComponent, CharacterComponent>());
        [DI] private readonly ProtoItCached _playerIt = new ProtoItCached(It.Inc<PlayerMarker, CharacterComponent>());

        public void Run()
        {
            _playerIt.BeginCaching();

            var (playerEntity, ok) = _playerIt.FirstSlow();
            if (ok == false)
            {
                return;
            }
            
            ref var playerCharacter = ref _gameAspect.CharacterPool.Get(playerEntity);
            
            foreach (ProtoEntity entity in _aiIt)
            {
                ref var moveComponent = ref _gameAspect.MovePool.Get(entity);
                ref var aiCharacter = ref _gameAspect.CharacterPool.Get(entity);
                
                var direction = (playerCharacter.Position - aiCharacter.Position).normalized;
                moveComponent.Direction = new Vector2(direction.x, direction.z);
            }
            
            _playerIt.EndCaching();
        }
    }
}