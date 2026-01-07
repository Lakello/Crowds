namespace _SOURCE_.Scripts.ECS.Aspects
{
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;

    public class GenericAspect<TComponent> : ProtoAspectInject 
        where TComponent : struct
    {
        public ProtoPool<TComponent> Pool;
    }
}