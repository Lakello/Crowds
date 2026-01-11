namespace TargetModule
{
    using System;
    using System.Collections.Generic;
    using Condition;
    using Sirenix.OdinInspector;
    using UnityEngine;

    public partial class TargetPoint : MonoBehaviour
    {
        [SerializeField]
        [BoxGroup("General")]
        private Vector3 _offset;

        [SerializeField]
        [Range(0, 1)]
        [BoxGroup("General")]
        private float _weight = 1;

        [SerializeReference]
        [BoxGroup("General")]
        private ITargetPointMode _targetPointMode = new PointTargetMode();

        [SerializeField]
        [BoxGroup("General")]
        private PointType _pointType;

        [SerializeField]
        [BoxGroup("General")]
        [ShowIf("IsDynamic")]
        private Vector3 _dynamicAxis;

        [SerializeField]
        [ShowIf(nameof(IsPoints))]
        [BoxGroup("General")]
        private List<TargetPoint> _points;

        [SerializeReference]
        [BoxGroup("Condition")]
        private ITargetPointCondition[] _nextConditions = Array.Empty<ITargetPointCondition>();

        [SerializeReference]
        [BoxGroup("Condition")]
        private ITargetPointCondition[] _skipConditions = Array.Empty<ITargetPointCondition>();

        private TargetPointData _selfTargetPointData = TargetPointData.Empty;

        private TargetPointData _currentTargetPointData = TargetPointData.Empty;

        [field: SerializeField]
        [field: BoxGroup("General")]
        public bool IsChild { get; private set; }

        public ref TargetPointData CurrentTargetPointData => ref GetCurrentData();

        public ref TargetPointData SelfTargetPointData => ref GetSelfPointData();

        private bool IsPoints => _targetPointMode
            is SequenceTargetMode
            or RandomPointTargetMode;

        private bool IsDynamic => _pointType == PointType.Dynamic;

        public bool CanNext()
        {
            foreach (var condition in _nextConditions)
            {
                if (condition.Check() == false)
                {
                    return false;
                }
            }

            return true;
        }

        public bool CanSkip()
        {
            foreach (var condition in _skipConditions)
            {
                if (condition.Check())
                {
                    return true;
                }
            }

            return false;
        }

        public TargetPointData GetData()
        {
            var data = _targetPointMode.Construct(CreateData(), this);
            data.UpdateCurrentPosition();

            _currentTargetPointData = data;

            return data;
        }

        private ref TargetPointData GetCurrentData()
        {
            if (_currentTargetPointData.IsNull)
            {
                _currentTargetPointData = GetData();
            }

            return ref _currentTargetPointData;
        }

        private ref TargetPointData GetSelfPointData()
        {
            if (_selfTargetPointData.IsNull)
            {
                var mode = _targetPointMode.GetSelfMode();
                _selfTargetPointData = CreateData();
                _selfTargetPointData.TargetPointMode = mode;
                _selfTargetPointData = mode.Construct(_selfTargetPointData, this);
            }

            _selfTargetPointData.UpdateCurrentPosition();

            return ref _selfTargetPointData;
        }

        private TargetPointData CreateData()
        {
            return new TargetPointData
            {
                Offset = _offset,
                Weight = _weight,
                PointType = _pointType,
                DynamicAxis = _dynamicAxis,
                TargetPointMode = _targetPointMode,
                SelfTargetPoint = this,
            };
        }
    }
}