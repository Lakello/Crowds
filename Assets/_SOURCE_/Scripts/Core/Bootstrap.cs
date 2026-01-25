namespace Core
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
	using UtilsModule.Execute.Mode;
	using UtilsModule.Other;

	public class Bootstrap : MonoBehaviour, IExecuteHolder
	{
		[SerializeField] private GameConfig _gameConfig;

		public ExecuteMethod Method => ExecuteMethod.Awake;
		public int Priority { get; set; }

		public Executor GetExecutor()
		{
			return new ExecutorAsync(Init);
		}

		private async UniTask Init(CancellationToken token)
		{
			DI.Register(_gameConfig);
			DI.Register(new GameInput());

			await PlatformProvider.WaitInitSDK(token);

			SceneManager.LoadScene(nameof(SceneName.Game));
		}
	}
}