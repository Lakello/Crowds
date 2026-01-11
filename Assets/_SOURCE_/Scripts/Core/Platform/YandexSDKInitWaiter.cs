namespace Core.Platform
{
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using YG;

    public class YandexSDKInitWaiter : ISDKInitWaiter
    {
        public async UniTask Wait(CancellationToken token)
        {
            if (YG2.isSDKEnabled)
            {
                return;
            }
            
            await UniTask.WaitUntil(() => YG2.isSDKEnabled, cancellationToken: token);
        }
    }
}