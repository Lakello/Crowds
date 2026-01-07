namespace UtilsModule.Other
{
    using System;

    public class ContractHolder<T> : IDisposable
    {
        private readonly T _obj;
        private readonly Action _dispose;

        public ContractHolder(T obj, Action dispose)
        {
            _obj = obj;
            _dispose = dispose;
        }
        
        public object Target => _obj;

        public void Dispose()
        {
            _dispose();
        }
    }
}