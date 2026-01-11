namespace UtilsModule.Other
{
	using System.Collections.Generic;
	using UnityEngine;

	public class AnimatorParamChecker
	{
		private Animator _animator;
		private HashSet<string> _parameterNames;
		private HashSet<int> _parameterHashNames;
		
		public bool HasParameter(string name)
		{
			return _parameterNames?.Contains(name) ?? false;
		}
		
		public bool HasParameter(int hash)
		{
			return _parameterHashNames?.Contains(hash) ?? false;
		}
		
		public void RefreshParameters(Animator animator)
		{
			if (_animator != animator)
			{
				_animator = animator;
				CacheParameterNames();
			}
		}

		private void CacheParameterNames()
		{
			_parameterNames = new HashSet<string>();
			_parameterHashNames = new HashSet<int>();
			if (_animator != null)
			{
				foreach (var param in _animator.parameters)
				{
					_parameterNames.Add(param.name);
					_parameterHashNames.Add(param.nameHash);
				}
			}
		}
	}
}