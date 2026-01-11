namespace ECS.Systems.Game
{
    using Aspects;
    using Authoring;
    using Authoring.Helpers;
    using global::Game.CharacterSystem;
    using global::Game.CharacterSystem.Data;
    using global::Game.Data;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using Services;
    using UnityEngine;

    public class FactorySystem : IProtoRunSystem
    {
        [DI] private CharacterPoolService _characterPoolService;
        [DI] private CharacterDataHolder _characterDataHolder;
        [DI] private PrefabsHolder _prefabsHolder;

        [DI] private GameAspect _gameAspect;

        [DI] private ProtoIt _factoryIt = new ProtoIt(It.Inc<FactoryComponent, TimerComponent>());

        public void Run()
        {
            foreach (ProtoEntity factoryEntity in _factoryIt)
            {
                ref TimerComponent timer = ref _gameAspect.TimerPool.Get(factoryEntity);
                ref FactoryComponent factory = ref _gameAspect.FactoryPool.Get(factoryEntity);
                ref EntityHolderComponent entityHolder = ref GetEntityHolderComponent(factoryEntity);

                if (timer.RemainingTime > 0
                    || entityHolder.Entities.Length >= factory.MaxCount)
                {
                    continue;
                }

                CharacterData characterData = _characterDataHolder.GetData(factory.Prefab.UnitType);

                for (int i = 0; i < factory.SpawnCount; i++)
                {
                    if (entityHolder.Entities.Length >= factory.MaxCount)
                    {
                        break;
                    }

                    Character character = null;

                    if (_characterPoolService.Pool[factory.Prefab.UnitType].Count == 0)
                    {
                        character = Object.Instantiate(factory.Prefab, factory.SpawnPoint.TargetPosition, Quaternion.identity);
                    }
                    else
                    {
                        character = _characterPoolService.Pool[factory.Prefab.UnitType].Dequeue();
                    }

                    _gameAspect.AiPool.NewEntity(out ProtoEntity characterEntity);

                    characterEntity.CreateCharacter(_gameAspect.CharacterPool, character);
                    characterEntity.CreateMove(_gameAspect.MovePool, character.transform, characterData);
                    characterEntity.CreateHealth(_gameAspect.HealthPool, characterData);

                    entityHolder.Entities.Add(_gameAspect.World().PackEntity(characterEntity));
                }
            }

            return;

            ref EntityHolderComponent GetEntityHolderComponent(ProtoEntity entity)
            {
                if (_gameAspect.SpawnedEntityHolderPool.Has(entity))
                {
                    return ref _gameAspect.SpawnedEntityHolderPool.Get(entity);
                }

                ref EntityHolderComponent entityHolderComponent = ref _gameAspect.SpawnedEntityHolderPool.Add(entity);
                entityHolderComponent.Init();

                return ref entityHolderComponent;
            }
        }
    }
}