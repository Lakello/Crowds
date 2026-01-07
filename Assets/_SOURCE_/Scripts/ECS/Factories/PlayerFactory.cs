namespace _SOURCE_.Scripts.ECS.Factories
{
    using Aspects;
    using Components;
    using Components.Marker;
    using Data;
    using Game.CharacterSystem;
    using Game.Data;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.Unity;
    using UnityEngine;
    using UtilsModule.Other;
    using UtilsModule.Singleton;

    public class PlayerFactory : IEventHandler
    {
        public void HandleEvent()
        {
            ProtoWorld world = ProtoUnityWorlds.Get();
            GameAspect gameAspect = (GameAspect)world.Aspect(typeof(GameAspect));

            PrefabsHolder holder = DI.Resolve<PrefabsHolder>();

            Character player = Object.Instantiate(
                holder.PlayerPrefab, 
                SingleAccessHolder.Instance.Get<SpawnPoint>().transform.position, Quaternion.identity);

            ref CharacterComponent characterComponent = ref gameAspect.CharacterPool.NewEntity(out ProtoEntity playerEntity);
            characterComponent.Character = player;

            gameAspect.PlayerPool.Add(playerEntity);
            
            ref MoveComponent moveComponent = ref gameAspect.MovePool.Add(playerEntity);
            moveComponent.Target = player.transform;
            moveComponent.MovementType = MovementType.Player;
        }
    }
}