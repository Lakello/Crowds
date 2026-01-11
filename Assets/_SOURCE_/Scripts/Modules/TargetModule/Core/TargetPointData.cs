namespace TargetModule
{
    using System;
    using System.Data.SqlTypes;
    using UnityEngine;

    [Serializable]
    public struct TargetPointData : INullable
    {
        private static TargetPointData s_emptyTargetPointData = new TargetPointData
        {
            IsNull = true,
            Points = Array.Empty<TargetPointData>(),
            TargetPointMode = new TargetPoint.EmptyTargetMode()
        };

        [SerializeField] private Vector3 _currentPosition;

        public static ref TargetPointData Empty => ref s_emptyTargetPointData;

        public TargetPointData[] Points { get; set; }
        public TargetPoint.ITargetPointMode TargetPointMode { get; set; }
        [field: SerializeField] public TargetPoint SelfTargetPoint { get; set; }
        [field: SerializeField] public int CurrentIndex { get; private set; }

        public bool IsNull { get; set; }
        public float Weight { get; set; }
        public Vector3 Offset { get; set; }
        public PointType PointType { get; set; }
        public Vector3 DynamicAxis { get; set; }
        public bool IsLoopSequence { get; set; }

        public TargetPoint TargetPoint => Points == null || Points.Length == 0
            ? SelfTargetPoint
            : Points[CurrentIndex].SelfTargetPoint;
        public Vector3 TargetPosition => PointType == PointType.Static
            ? _currentPosition
            : GetNewCurrentPosition();
        public Vector3 SelfPosition => SelfTargetPoint.SelfTargetPointData.TargetPosition;
        public bool IsEndSequence => Points is { Length: > 0 } && CurrentIndex == Points.Length - 1;

        public bool TryNext()
        {
            if (TargetPointMode.CanNext(ref this) == false)
                return false;

            if (TargetPoint.CanNext())
            {
                TrySkip();

                Next();

                return true;
            }

            UpdateCurrentPosition();

            return false;
        }

        public bool TrySkip()
        {
            if (TargetPointMode.CanSkip(ref this) == false)
                return false;

            if (TargetPoint.CanSkip())
            {
                Next();

                return true;
            }

            return false;
        }

        public void ResetSequence()
        {
            CurrentIndex = 0;
        }

        public void UpdateCurrentPosition()
        {
            GetNewCurrentPosition();

            if (Points is { Length: > 0 })
            {
                Points[CurrentIndex].UpdateCurrentPosition();
            }
        }

        private Vector3 GetNewCurrentPosition()
        {
            _currentPosition = TargetPointMode.GetTargetPosition(ref this) + Offset;
            return _currentPosition;
        }

        private void Next()
        {
            CurrentIndex++;

            if (CurrentIndex >= Points.Length)
            {
                if (IsLoopSequence)
                    CurrentIndex = 0;
                else
                    CurrentIndex = Points.Length - 1;
            }

            UpdateCurrentPosition();
        }
    }
}