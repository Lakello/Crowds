namespace TargetModule
{
	using UnityEngine;

	public partial class TargetPoint
	{
		public class PointTargetMode : ITargetPointMode
		{
			private Transform _transform;

			public TargetPointData Construct(TargetPointData data, TargetPoint targetPoint)
			{
				_transform = targetPoint.transform;

				return data;
			}

			public Vector3 GetTargetPosition(ref TargetPointData data)
			{
				if (_transform == null)
				{
					return Vector3.zero;
				}

				return _transform.position;
			}

			public bool CanNext(ref TargetPointData data)
			{
				return false;
			}

			public bool CanSkip(ref TargetPointData data)
			{
				return false;
			}

			public ITargetPointMode GetSelfMode()
			{
				return this;
			}
		}
	}
}