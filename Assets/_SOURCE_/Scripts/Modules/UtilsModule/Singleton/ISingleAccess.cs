namespace UtilsModule.Singleton
{
    using System;

    public interface ISingleAccess
    {
        public Type TargetType { get; }
    }
}