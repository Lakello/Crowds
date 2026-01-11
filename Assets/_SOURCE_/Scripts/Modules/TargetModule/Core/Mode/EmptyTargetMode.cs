namespace TargetModule
{
	using UnityEngine;

	public partial class TargetPoint
	{
		public class EmptyTargetMode : ITargetPointMode
		{
			public TargetPointData Construct(TargetPointData data, TargetPoint targetPoint)
			{
				return data;
			}

			public Vector3 GetTargetPosition(ref TargetPointData data)
			{
				return Vector3.zero;
			}

			public bool CanNext(ref TargetPointData data)
			{
				return true;
			}

			public bool CanSkip(ref TargetPointData data)
			{
				return true;
			}

			public ITargetPointMode GetSelfMode()
			{
				return this;
			}
		}
	}
}