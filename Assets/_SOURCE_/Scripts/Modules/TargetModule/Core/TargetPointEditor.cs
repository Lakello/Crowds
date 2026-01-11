#if UNITY_EDITOR
namespace TargetModule
{
	using System.Linq;
	using Sirenix.OdinInspector;
	using UnityEditor;
	using UnityEngine;

	public partial class TargetPoint
	{
		[SerializeField]
		[BoxGroup("Editor")]
		private bool _isShowTargetPoint;

		[SerializeField]
		[ShowIf(nameof(IsPoints))]
		[BoxGroup("Editor")]
		private Transform _parent;

		[SerializeField]
		[ShowIf(nameof(IsPoints))]
		[BoxGroup("Editor")]
		private bool _canAddThis = true;

		[SerializeField]
		[ShowIf(nameof(IsPoints))]
		[BoxGroup("Editor")]
		private bool _canMarkChild = true;

		private Vector3 _targetPointShowPosition;

		private void OnValidate()
		{
			if (_parent == null)
			{
				_parent = transform;
				_canAddThis = true;
				_canMarkChild = true;
			}
		}

		[Button]
		[BoxGroup("Editor")]
		private void ShowTargetPoint()
		{
			_isShowTargetPoint = true;
			_targetPointShowPosition = GetData().TargetPosition;
		}

		[Button]
		[ShowIf(nameof(IsPoints))]
		[BoxGroup("Editor")]
		private void FindPoints()
		{
			_points = _parent.GetComponentsInChildren<TargetPoint>()
				.Where(p => p.transform.parent.TryGetComponent(out TargetPoint targetPoint) && targetPoint == this)
				.ToList();

			if (_points is { Count: > 0 })
			{
				if (_points[0] == this)
				{
					if (_canAddThis)
					{
						if (_points.Count > 1)
						{
							_points.Remove(this);
							_points.Add(this);
						}
					}
					else
					{
						_points.Remove(this);
					}
				}
			}

			if (_canMarkChild)
			{
				foreach (var point in _points)
				{
					if (point != this)
					{
						point.IsChild = true;
						EditorUtility.SetDirty(point);
					}
				}
			}

			EditorUtility.SetDirty(this);
		}
		
		[Button]
		[ShowIf(nameof(IsPoints))]
		[BoxGroup("Editor")]
		private void FindPointsInChildren()
		{
			_points = _parent.GetComponentsInChildren<TargetPoint>()
				.ToList();

			if (_points is { Count: > 0 })
			{
				if (_points[0] == this)
				{
					if (_canAddThis)
					{
						if (_points.Count > 1)
						{
							_points.Remove(this);
							_points.Add(this);
						}
					}
					else
					{
						_points.Remove(this);
					}
				}
			}

			if (_canMarkChild)
			{
				foreach (var point in _points)
				{
					if (point != this)
					{
						point.IsChild = true;
						EditorUtility.SetDirty(point);
					}
				}
			}

			EditorUtility.SetDirty(this);
		}

		private void OnDrawGizmos()
		{
			if (_isShowTargetPoint)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere(_targetPointShowPosition, 0.2f);
			}

			if (_targetPointMode is TargetPoint.RangeTargetMode == false)
			{
				return;
			}

			Gizmos.color = Color.blue;
			Gizmos.DrawWireCube(transform.position + _offset, _size);
		}
	}
}
#endif