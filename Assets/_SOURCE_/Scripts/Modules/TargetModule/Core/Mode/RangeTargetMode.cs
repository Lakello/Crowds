namespace TargetModule
{
    using Sirenix.OdinInspector;
    using UnityEngine;

    public partial class TargetPoint
    {
        [SerializeField] 
        [ShowIf(nameof(IsRange))]
        [BoxGroup("General")]
        private Vector3 _size;
        
        private bool IsRange => _targetPointMode is RangeTargetMode;
        
        public class RangeTargetMode : ITargetPointMode
        {
            private Range _range;
            
            private TargetPoint _targetPoint;

            public TargetPointData Construct(TargetPointData data, TargetPoint targetPoint)
            {
                _targetPoint = targetPoint;
                
                CalculateRange(Vector3.one);

                return data;
            }

            public Vector3 GetTargetPosition(ref TargetPointData data)
            {
                if (data.PointType == PointType.Dynamic)
                {
                    CalculateRange(data.DynamicAxis);
                }
                
                return _range.GetRandomPosition();
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

            private void CalculateRange(Vector3 axis)
            {
                _range.CalculateRange(_targetPoint.transform.position, _targetPoint._size, axis);
            }

            private struct Range
            {
                private Vector2 _x;
                private Vector2 _y;
                private Vector2 _z;

                public Vector3 GetRandomPosition()
                {
                    return new Vector3(
                        Random.Range(_x.x, _x.y),
                        Random.Range(_y.x, _y.y),
                        Random.Range(_z.x, _z.y));
                }

                public void CalculateRange(Vector3 center, Vector3 size, Vector3 axis)
                {
                    if (axis.x > 0)
                    {
                        _x.x = center.x - size.x / 2;
                        _x.y = center.x + size.x / 2;
                    }

                    if (axis.y > 0)
                    {
                        _y.x = center.y - size.y / 2;
                        _y.y = center.y + size.y / 2;
                    }
                    
                    if (axis.z > 0)
                    {
                        _z.x = center.z - size.z / 2;
                        _z.y = center.z + size.z / 2;
                    }
                }
            }
        }
    }
}