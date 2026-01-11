namespace ECS.Extensions
{
    using Leopotam.EcsProto;

    public static class ProtoExtensions
    {
        public static T Service<T>(this IProtoSystems systems)
        {
            return (T)systems.Services()[typeof(T)];
        }
    }
}