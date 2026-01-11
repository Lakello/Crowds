namespace Core.Platform
{
    using System.Threading;
    using Cysharp.Threading.Tasks;

    public interface ISDKInitWaiter
    {
        public UniTask Wait(CancellationToken token);
    }
}