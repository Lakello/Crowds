namespace ECS.Systems.Game
{
    using Aspects;
    using Authoring;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;

    public class DisposeSystem : IProtoDestroySystem
    {
        [DI] private GameAspect _gameAspect;

        [DI] private ProtoIt _disposableIt = new ProtoIt(It.Inc<EntityHolderComponent>());

        public void Destroy()
        {
            foreach (ProtoEntity disposableEntity in _disposableIt)
            {
                if (_gameAspect.SpawnedEntityHolderPool.Has(disposableEntity))
                {
                    ref EntityHolderComponent entityHolderComponent = ref _gameAspect.SpawnedEntityHolderPool.Get(disposableEntity);
                    entityHolderComponent.Dispose();
                }
            }
        }
    }
}