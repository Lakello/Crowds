namespace ECS.Authoring
{
    using System;
    using Leopotam.EcsProto.Unity;

    [Serializable]
    [ProtoUnityAuthoring("Timer")]
    public struct TimerComponent
    {
        public float Time;
        public float RemainingTime;
    }
}