namespace ECS.Authoring
{
    using System;
    using Game.CharacterSystem;
    using Leopotam.EcsProto.QoL;
    using Leopotam.EcsProto.Unity;
    using TargetModule;
    using UnityEngine;

    [Serializable]
    [ProtoUnityAuthoring("Factory")]
    public struct FactoryComponent : IProtoUnityAuthoring
    {
        public int SpawnedCount;

        public int SpawnCount;
        public int MaxCount;
        public Character Prefab;
        public TargetPointData SpawnPoint;

        public void Authoring(in ProtoPackedEntityWithWorld entity, GameObject go)
        {
            SpawnPoint = go.GetComponent<TargetPoint>().GetData();
        }
    }
}