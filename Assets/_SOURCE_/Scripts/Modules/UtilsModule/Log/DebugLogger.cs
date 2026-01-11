namespace UtilsModule.Log
{
	using UnityEngine;

	public class DebugLogger
	{
		private string _prefix;

		public DebugLogger(string prefix, bool isEnabled = true)
		{
			_prefix = prefix;
			IsEnabled = isEnabled;
		}
		
		public bool IsEnabled { get; }

		public string GetMessage(string message)
		{
			return $"[{_prefix}] {message}";
		}
		
		public void Send(string message)
		{
			if (IsEnabled == false)
			{
				return;
			}
			Debug.Log(GetMessage(message));
		}
		
		public void SendWarning(string message)
		{
			if (IsEnabled == false)
			{
				return;
			}
			Debug.LogWarning(GetMessage(message));
		}
		
		public void SendError(string message)
		{
			if (IsEnabled == false)
			{
				return;
			}
			Debug.LogError(GetMessage(message));
		}
	}
}