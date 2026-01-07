namespace _SOURCE_.Scripts.Core
{
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using Game.Data;
    using Platform;
    using SceneSystem;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UtilsModule.Execute;
    using UtilsModule.Execute.Interfaces;
    using UtilsModule.Other;

    public class Bootstrap : MonoBehaviour, IExecuteHolder
    {
        [SerializeField] private PrefabsHolder _prefabsHolder;

        public ExecuteMethod Method => ExecuteMethod.Awake;
        public int Priority { get; set; }

        public Executor GetExecutor()
        {
            return new ExecutorAsync(Init);
        }

        private async UniTask Init(CancellationToken token)
        {
            DI.Register(_prefabsHolder);

            await PlatformProvider.WaitInitSDK(token);

            SceneManager.LoadScene(nameof(SceneName.Game));
        }
    }
}