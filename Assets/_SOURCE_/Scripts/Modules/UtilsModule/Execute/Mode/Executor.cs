namespace UtilsModule.Execute
{
	using System.Threading;
	using Cysharp.Threading.Tasks;

	public abstract class Executor
	{
		public abstract ExecuteMode Mode { get; }

		public virtual void Execute()
		{
		}
		
		public virtual UniTask ExecuteAsync(CancellationToken token)
		{
			return UniTask.CompletedTask;
		}
	}
}