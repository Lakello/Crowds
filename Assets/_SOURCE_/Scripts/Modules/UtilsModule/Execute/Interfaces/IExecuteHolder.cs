namespace UtilsModule.Execute.Interfaces
{
	using Mode;

	public interface IExecuteHolder
	{
		public ExecuteMethod Method { get; }
		public int Priority { get; set; }
		
		public Executor GetExecutor();
	}
}