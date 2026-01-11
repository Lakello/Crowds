namespace ECS.Authoring
{
    using System;
    using Leopotam.EcsProto.QoL;
    using Unity.Collections;

    public struct EntityHolderComponent : IDisposable
    {
        public NativeList<ProtoPackedEntity> Entities;

        public void Init()
        {
            Dispose();
            
            Entities = new NativeList<ProtoPackedEntity>(Allocator.Persistent);
        }
        
        public void Dispose()
        {
            if (Entities.IsCreated)
            {
                Entities.Dispose();
            }
        }
    }
}