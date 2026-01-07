namespace UtilsModule.Singleton
{
    using System;
    using System.Collections.Generic;

    public class SingleAccessHolder : SingletonMono<SingleAccessHolder>
    {
        private Dictionary<Type, ISingleAccess> _singleAccesses = new Dictionary<Type, ISingleAccess>();

        public T Get<T>()
        {
            if (_singleAccesses.ContainsKey(typeof(T)))
            {
                return (T)_singleAccesses[typeof(T)];
            }
            
            throw new Exception($"No single access {typeof(T).Name}");
        }
        
        protected override void InternalInit()
        {
            ISingleAccess[] accesses = GetComponentsInChildren<ISingleAccess>();

            foreach (ISingleAccess singleAccess in accesses)
            {
                _singleAccesses.Add(singleAccess.TargetType, singleAccess);
            }
        }
    }
}