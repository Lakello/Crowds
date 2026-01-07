namespace _SOURCE_.Scripts.Core.Platform
{
    using System.Threading;
    using Cysharp.Threading.Tasks;

    public static class PlatformProvider
    {
        public static async UniTask WaitInitSDK(CancellationToken token)
        {
            ISDKInitWaiter waiter = null;

#if UNITY_WEBGL
            waiter = new YandexSDKInitWaiter();
#endif

            await waiter.Wait(token);
        }
    }
}