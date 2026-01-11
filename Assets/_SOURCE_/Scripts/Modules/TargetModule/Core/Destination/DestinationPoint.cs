namespace TargetModule.Destination
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sirenix.OdinInspector;
    using UnityEngine;
    using Random = UnityEngine.Random;
    using TargetPoint = TargetModule.TargetPoint;

    [Serializable]
    public class DestinationPoint : IDestinationPoint
    {
        private readonly List<TargetPointData> _points;

        [ShowInInspector] private TargetPointData _currentData;

        private SelectTargetMode _selectTargetMode;
        private float _totalWeight;
        private int _currentIndex = -1;

        public DestinationPoint(SelectTargetMode selectTargetMode, params TargetPoint[] points)
        {
            _points = points.Select(p => p.GetData()).ToList();

            _selectTargetMode = selectTargetMode;

            _totalWeight = _points.Sum(p => p.Weight);

            SetCurrent(0);

            NextData();
        }

        [ShowInInspector] private int CurrentIndex => _currentIndex;

        public ref TargetPointData GetPreviousTargetData()
        {
            SetCurrent(_currentIndex - 1);
            return ref _currentData;
        }

        public ref TargetPointData GetCurrentTargetData()
        {
            return ref _currentData;
        }

        public void NextData()
        {
            if (_currentData.TryNext() == false)
            {
                NextPoint();
            }

            _currentData.TrySkip();

            _currentData.UpdateCurrentPosition();
        }

        public void NextPoint()
        {
            switch (_selectTargetMode)
            {
                case SelectTargetMode.Order:
                case SelectTargetMode.OrderLoop:
                    SetOrderPoint();

                    break;
                case SelectTargetMode.Random:
                    SetRandomPoint();

                    break;
            }
        }

        public void SetPoint(int point)
        {
#if UNITY_EDITOR
            int count = _points.Count;
            if (count == 0) return;

            point = ((point % count) + count) % count;
#endif

            SetCurrent(point);
        }

        public void ResetPoint()
        {
            SetCurrent(0);
        }

        private void SetOrderPoint()
        {
            int index = _currentIndex + 1 >= _points.Count
                ? _selectTargetMode == SelectTargetMode.OrderLoop
                    ? 0
                    : _points.Count - 1
                : _currentIndex + 1;

            SetCurrent(index);
        }

        private void SetRandomPoint()
        {
            var random = Random.Range(0f, _totalWeight);

            for (int i = 0; i < _points.Count; i++)
            {
                if (random < _points[i].Weight)
                {
                    SetCurrent(i);
                    return;
                }

                random -= _points[i].Weight;
            }

            SetCurrent(Random.Range(0, _points.Count));
        }

        private void SetCurrent(int index)
        {
            if (_points.Count == 0)
            {
                return;
            }

            if (_currentIndex >= 0 && _currentIndex < _points.Count)
            {
                _points[_currentIndex].ResetSequence();
            }

            _currentIndex = Mathf.Clamp(index, 0, _points.Count - 1);
            _currentData = _points[_currentIndex];

            _currentData.UpdateCurrentPosition();
        }
    }
}