namespace TargetModule
{
	using System.Linq;
	using Sirenix.OdinInspector;
	using UnityEngine;

	public partial class TargetPoint
	{
		[SerializeField]
		[ShowIf(nameof(IsSequence))]
		[BoxGroup("General")]
		private bool _isLoopSequence;

		private bool IsSequence => _targetPointMode is SequenceTargetMode;

		public class SequenceTargetMode : ITargetPointMode
		{
			public TargetPointData Construct(TargetPointData data, TargetPoint targetPoint)
			{
				data.IsLoopSequence = targetPoint._isLoopSequence;

				data.Points = targetPoint._points
					.Select(p => targetPoint == p
						? p.GetSelfPointData()
						: p.GetData())
					.ToArray();

				return data;
			}

			public Vector3 GetTargetPosition(ref TargetPointData data)
			{
				return data.Points[data.CurrentIndex].TargetPosition;
			}

			public bool CanNext(ref TargetPointData data)
			{
				return data.IsEndSequence == false;
			}

			public bool CanSkip(ref TargetPointData data)
			{
				return data.IsEndSequence == false;
			}

			public ITargetPointMode GetSelfMode()
			{
				return new PointTargetMode();
			}
		}
	}
}