namespace ECS.Factories
{
    using Aspects;
    using Authoring.Helpers;
    using Game.CharacterSystem;
    using Game.CharacterSystem.Data;
    using Game.Data;
    using Leopotam.EcsProto;
    using UnityEngine;
    using UtilsModule.Other;
    using UtilsModule.Singleton;

    public class PlayerFactory
    {
        public static void Create(GameAspect gameAspect, CharacterData characterData)
        {
            PrefabsHolder holder = DI.Resolve<GameConfig>().PrefabsHolder;

            Character player = Object.Instantiate(
                holder.PlayerPrefab,
                SingleAccessHolder.Instance.Get<SpawnPoint>().transform.position, Quaternion.identity);

            gameAspect.PlayerPool.NewEntity(out ProtoEntity playerEntity);

            playerEntity.CreateCharacter(gameAspect.CharacterPool, player);
            playerEntity.CreateMove(gameAspect.MovePool, player.transform, characterData);
            playerEntity.CreateHealth(gameAspect.HealthPool, characterData);
        }
    }
}