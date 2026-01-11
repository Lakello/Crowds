namespace TargetModule
{
    using System;
    using System.Linq;
    using Sirenix.OdinInspector;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public partial class TargetPoint
    {
        [SerializeField]
        [ShowIf("IsRandomPoint")]
        private RandomPointTargetModeData _data;

        [HideLabel]
        [Serializable]
        [BoxGroup("General")]
        private struct RandomPointTargetModeData
        {
            [SerializeField]
            public bool OnlyActivePoints;
            [SerializeField]
            public bool UseWeight;
            [SerializeField]
            [ShowIf("UseWeight")]
            public bool UseShuffle;
        }

        private bool IsRandomPoint => _targetPointMode is RandomPointTargetMode;

        public class RandomPointTargetMode : ITargetPointMode
        {
            private bool _onlyActivePoints;
            private bool _useWeight;
            private bool _useShuffle;

            public TargetPointData Construct(TargetPointData data, TargetPoint targetPoint)
            {
                _onlyActivePoints = targetPoint._data.OnlyActivePoints;
                _useWeight = targetPoint._data.UseWeight;
                _useShuffle = targetPoint._data.UseShuffle;

                data.Points = targetPoint._points.Select(p => p.GetData()).ToArray();
                return data;
            }

            public Vector3 GetTargetPosition(ref TargetPointData data)
            {
                return GetData(ref data).TargetPosition;
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

            private ref TargetPointData GetData(ref TargetPointData data)
            {
                var points = _onlyActivePoints
                    ? GetActivePoints(ref data)
                    : data.Points;

                if (points == null
                    || points.Length == 0)
                {
                    points = new[]
                    {
                        TargetPointData.Empty
                    };

                    return ref points[0];
                }

                if (points.Length == 1)
                {
                    return ref points[0];
                }

                if (_useWeight)
                {
                    Chance.WeightChanceData<int>[] weightData = new Chance.WeightChanceData<int>[points.Length];

                    for (int i = 0; i < weightData.Length; i++)
                    {
                        weightData[i] = new Chance.WeightChanceData<int>
                        {
                            Data = i,
                            Weight = points[i].Weight
                        };
                    }

                    return ref points[Chance.FromWeight(weightData, useShuffle: _useShuffle)];
                }

                return ref points[Random.Range(0, points.Length)];
            }

            private TargetPointData[] GetActivePoints(ref TargetPointData data)
            {
                if (CheckArray(ref data))
                {
                    return data.Points
                        .Where(p =>
                        {
                            if (p.SelfTargetPoint == null
                                || p.SelfTargetPoint.gameObject == null)
                            {
                                return false;
                            }

                            return p.SelfTargetPoint.gameObject.activeSelf;
                        })
                        .ToArray();
                }

                return new[]
                {
                    TargetPointData.Empty
                };

                bool CheckArray(ref TargetPointData data)
                {
                    return data.Points != null;
                }
            }
        }
    }
}